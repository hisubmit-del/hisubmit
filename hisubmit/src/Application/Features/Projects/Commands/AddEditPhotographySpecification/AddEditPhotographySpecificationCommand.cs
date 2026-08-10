using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditPhotographySpecification
{
    public class AddEditPhotographySpecificationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public string Genre { get; set; }
        public DateTime? TakenDate { get; set; }
        public int OriginCountryId { get; set; }
        public string Camera { get; set; }
        public string Lens { get; set; }
        public string FocalLength { get; set; }
        public string ShutterSpeed { get; set; }
        public string Aperture { get; set; }
        public string Iso_Film { get; set; }
        public string Location { get; set; }
        public bool StudentProject { get; set; }

        public List<int> SubProjectTypeIds { get; set; }

        //navigation propety
        public int ProjectId { get; set; }

        public AddEditPhotographySpecificationCommand()
        {
            SubProjectTypeIds = new List<int>();
            TakenDate = DateTime.Now;
        }
    }
    internal class AddEditPhotographySpecificationCommandHandler : IRequestHandler<AddEditPhotographySpecificationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICheckPermission _checkPermission;
        private readonly IStringLocalizer<AddEditPhotographySpecificationCommandHandler> _localizer;

        public AddEditPhotographySpecificationCommandHandler
            (IMapper mapper, IUnitOfWork<int> unitOfWork,
                ICheckPermission checkPermission, 
                IStringLocalizer<AddEditPhotographySpecificationCommandHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _checkPermission = checkPermission;
            _localizer = localizer;
        }
        public async Task<Result<int>> Handle(AddEditPhotographySpecificationCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
            await _checkPermission.CheckWrightProjectPermission(project.UserId);


            if (request.Id == 0)
            {
                var specification = _mapper.Map<PhotographySpecification>(request);
                await _unitOfWork.Repository<PhotographySpecification>().AddAsync(specification);
                foreach (var subProjectids in request.SubProjectTypeIds)
                {
                    specification.PhotographySpecificationSubProjectTypes.Add(new PhotographySpecificationSubProjectType()
                    {
                        SubProjectTypeId = subProjectids
                    });
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(specification.Id, _localizer["Specification updated"]);
            }
            else
            {
                var dbSpecification = await _unitOfWork.Repository<PhotographySpecification>().GetByIdAsync(request.Id);
                if (dbSpecification != null)
                {
                    var updatedSpecification = _mapper.Map(request, dbSpecification);
                    await UpdatedSubProjectTypes(request.SubProjectTypeIds, request.Id);

                    await _unitOfWork.Repository<PhotographySpecification>().UpdateAsync(updatedSpecification);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync(dbSpecification.Id, _localizer["Specification updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(_localizer["Specification not found"]);
                }
            }
        }


        private async Task UpdatedSubProjectTypes(List<int> subProjectTypesId, int specificationId)
        {
            var dbspecificationProjectTypes = _unitOfWork.Repository<PhotographySpecificationSubProjectType>().Entities
                .Where(p => p.PhotographySpecificationId == specificationId);

            var deletedSpecificationProjectTypes = dbspecificationProjectTypes.Where(deadlneCat => !subProjectTypesId.Any(id => id == deadlneCat.Id))
               .ToList();

            var addedSpecificationProjectType = subProjectTypesId.Where(id => !dbspecificationProjectTypes.Any(focus => focus.Id == id))
                .ToList();

            if (deletedSpecificationProjectTypes != null)
            {
                foreach (var item in deletedSpecificationProjectTypes)
                {
                    await _unitOfWork.Repository<PhotographySpecificationSubProjectType>().DeleteAsync(item);
                }
            }
            if (addedSpecificationProjectType != null)
            {
                foreach (var item in addedSpecificationProjectType)
                {
                    await _unitOfWork.Repository<PhotographySpecificationSubProjectType>().AddAsync(new PhotographySpecificationSubProjectType()
                    {
                        PhotographySpecificationId = specificationId,
                        SubProjectTypeId = item
                    });
                }
            }
        }
    }
}
