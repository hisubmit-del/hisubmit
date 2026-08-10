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

namespace HiSubmit.Application.Features.Projects.Commands.AddEditVrXrSpecification
{
    public class AddEditVrXrSpecificationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }

        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }

        public bool VariableRunTime { get; set; }
        public string DescriptionRunTime { get; set; }
        public int MinRunTimeHours { get; set; }
        public int MinRunTimeMinutes { get; set; }
        public int MinRunTimeSecounds { get; set; }
        public int MaxTimeHours { get; set; }
        public int MaxTimeMinutes { get; set; }
        public int MaxTimeSecounds { get; set; }
        public int AvgTimeHours { get; set; }
        public int AvgTimeMinutes { get; set; }
        public int AvgTimeSecounds { get; set; }

        public DateTime? CompletionDate { get; set; }
        public int ProductionBudget { get; set; }

        public int OriginCountryId { get; set; }

        public string Language { get; set; }
        public bool StudentProject { get; set; }

        public int? MonetaryUnitId { get; set; }

        //navigationProperty
        public int ProjectId { get; set; }

        public AddEditVrXrSpecificationCommand()
        {
            CompletionDate = DateTime.Today;
            SubProjectTypeIds = new List<int>();
        }
    }

    public class AddEditVrXrSpecificationCommandHandler : IRequestHandler<AddEditVrXrSpecificationCommand, Result<int>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly ICheckPermission _checkPermission;
        private readonly IStringLocalizer<AddEditVrXrSpecificationCommandHandler> _localizer;

        public AddEditVrXrSpecificationCommandHandler
        (ICheckPermission checkPermission,
            IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditVrXrSpecificationCommandHandler> localizer)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _checkPermission = checkPermission;
            _localizer = localizer;
        }

        public async Task<Result<int>> Handle(AddEditVrXrSpecificationCommand request,
            CancellationToken cancellationToken)
        {
            var project = await _unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
            await _checkPermission.CheckWrightProjectPermission(project.UserId);
            if (request.Id == 0)
            {
                var specification = _mapper.Map<XrVrSpecification>(request);
                foreach (var subProjectids in request.SubProjectTypeIds)
                {
                    specification.ProjectType.Add(new SubProjectTypeVRXrSpecification()
                    {
                        SubProjectTypeId = subProjectids
                    });
                }

                await _unitOfWork.Repository<XrVrSpecification>().AddAsync(specification);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(specification.Id, _localizer["Specification updated"]);
            }
            else
            {
                var dbSpecification = await _unitOfWork.Repository<XrVrSpecification>().GetByIdAsync(request.Id);
                if (dbSpecification != null)
                {
                    var updatedSpecification = _mapper.Map(request, dbSpecification);
                    await UpdatedSubProjectTypes(request.SubProjectTypeIds, request.Id);
                    await _unitOfWork.Repository<XrVrSpecification>().UpdateAsync(updatedSpecification);
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
            var dbspecificationProjectTypes = _unitOfWork.Repository<SubProjectTypeVRXrSpecification>().Entities
                .Where(p => p.XrVrSpecificationId == specificationId);

            var deletedSpecificationProjectTypes = dbspecificationProjectTypes
                .Where(deadlneCat => !subProjectTypesId.Any(id => id == deadlneCat.Id))
                .ToList();

            var addedSpecificationProjectType = subProjectTypesId
                .Where(id => !dbspecificationProjectTypes.Any(focus => focus.Id == id))
                .ToList();

            if (deletedSpecificationProjectTypes != null)
            {
                foreach (var item in deletedSpecificationProjectTypes)
                {
                    await _unitOfWork.Repository<SubProjectTypeVRXrSpecification>().DeleteAsync(item);
                }
            }

            if (addedSpecificationProjectType != null)
            {
                foreach (var item in addedSpecificationProjectType)
                {
                    await _unitOfWork.Repository<SubProjectTypeVRXrSpecification>().AddAsync(
                        new SubProjectTypeVRXrSpecification()
                        {
                            XrVrSpecificationId = specificationId,
                            SubProjectTypeId = item
                        });
                }
            }
        }
    }
}