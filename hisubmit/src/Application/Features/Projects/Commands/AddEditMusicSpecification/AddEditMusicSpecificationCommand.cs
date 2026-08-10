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

namespace HiSubmit.Application.Features.Projects.Commands.AddEditMusicSpecification
{
    public class AddEditMusicSpecificationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }

        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int OriginCountryId { get; set; }

        public string Language { get; set; }

        public bool StudentProject { get; set; }

        //navigation Property
        public int ProjectId { get; set; }
        public AddEditMusicSpecificationCommand()
        {
            SubProjectTypeIds = new List<int>();
            CompletionDate = DateTime.Today;
        }
    }
    internal class AddEditMusicSpecificationCommandHandler : IRequestHandler<AddEditMusicSpecificationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICheckPermission _checkPermission;
        private readonly IStringLocalizer<AddEditMusicSpecificationCommandHandler> _localizer;

        public AddEditMusicSpecificationCommandHandler
            (IMapper mapper, IUnitOfWork<int> unitOfWork, 
                ICheckPermission checkPermission, 
                IStringLocalizer<AddEditMusicSpecificationCommandHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _checkPermission = checkPermission;
            _localizer = localizer;
        }
        public async Task<Result<int>> Handle(AddEditMusicSpecificationCommand request, CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
            await _checkPermission.CheckWrightProjectPermission(project.UserId);

            if (request.Id == 0)
            {
                var specification = _mapper.Map<MusicSpecification>(request);
                foreach (var subProjectids in request.SubProjectTypeIds)
                {
                    specification.ProjectType.Add(new SubProjectTypeMusicSpecification()
                    {
                        SubProjectTypeId = subProjectids
                    });
                }
                await _unitOfWork.Repository<MusicSpecification>().AddAsync(specification);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(specification.Id, _localizer["Specification updated"]);
            }
            else
            {
                var dbSpecification = await _unitOfWork.Repository<MusicSpecification>().GetByIdAsync(request.Id);
                if (dbSpecification != null)
                {
                    var updatedSpecification = _mapper.Map(request, dbSpecification);
                    await UpdatedSubProjectTypes(request.SubProjectTypeIds, request.Id);
                    await _unitOfWork.Repository<MusicSpecification>().UpdateAsync(updatedSpecification);
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
            var dbspecificationProjectTypes = _unitOfWork.Repository<SubProjectTypeMusicSpecification>().Entities
                .Where(p => p.MusicSpecificationId == specificationId);

            var deletedSpecificationProjectTypes = dbspecificationProjectTypes.Where(deadlneCat => !subProjectTypesId.Any(id => id == deadlneCat.Id))
               .ToList();

            var addedSpecificationProjectType = subProjectTypesId.Where(id => !dbspecificationProjectTypes.Any(focus => focus.Id == id))
                .ToList();

            if (deletedSpecificationProjectTypes != null)
            {
                foreach (var item in deletedSpecificationProjectTypes)
                {
                    await _unitOfWork.Repository<SubProjectTypeMusicSpecification>().DeleteAsync(item);
                }
            }
            if (addedSpecificationProjectType != null)
            {
                foreach (var item in addedSpecificationProjectType)
                {
                    await _unitOfWork.Repository<SubProjectTypeMusicSpecification>().AddAsync(new SubProjectTypeMusicSpecification()
                    {
                        MusicSpecificationId = specificationId,
                        SubProjectTypeId = item
                    });
                }
            }
        }
    }
}


