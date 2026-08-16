using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using HiSubmit.Domain.Entities.Locations;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Projects.Commands.AddEditFilmSpecification
{
    public class AddEditFilmSpecificationCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public List<int> SubProjectTypeIds { get; set; }
        public string Genre { get; set; }
        public int RunTimeHours { get; set; }
        public int RunTimeMinutes { get; set; }
        public int RunTimeSecounds { get; set; }
        public DateTime? CompletionDate { get; set; }
        public int? MonetaryUnitId { get; set; }

        public int ProductionBudget { get; set; }
        public int OriginCountryId { get; set; }
        public List<int> FilmingCountryIds { get; set; }
        public string Language { get; set; }
        public string ShottingFormat { get; set; }
        public string AspectRatio { get; set; }
        public FilmColor FilmColor { get; set; }
        public bool StudentProject { get; set; }
        public bool FirstTimeFilmMaker { get; set; }
        
        //navigation property
        public int ProjectId { get; set; }

        public AddEditFilmSpecificationCommand()
        {
            SubProjectTypeIds = new List<int>();
            CompletionDate = DateTime.Today;
        }
    }

    public class AddEditSubProjectTypeFilmSpecificationRequest
    {
        public int SubProjectTypeId { get; set; }
        public int FilmSpecificationId { get; set; }
    }

    public class AddEditFilmSpecificationCommandHandler(
        IMapper mapper,
        IUnitOfWork<int> unitOfWork,
        ICheckPermission checkPermission,
        IStringLocalizer<AddEditFilmSpecificationCommandHandler> localizer)
        : IRequestHandler<AddEditFilmSpecificationCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(AddEditFilmSpecificationCommand request, CancellationToken cancellationToken)
        {
            var project = await unitOfWork.Repository<Project>().GetByIdAsync(request.ProjectId);
            await checkPermission.CheckWrightProjectPermission(project.UserId);

            if (request.Id == 0)
            {           
                
                var specification = mapper.Map<FilmSpecification>(request);
                foreach (var subProjectIds in request.SubProjectTypeIds)
                {
                    specification.ProjectTypes.Add(new SubProjectTypeFilmSpecification()
                    {
                        SubProjectTypeId = subProjectIds
                    });
                }

                if (request.FilmingCountryIds is { Count: > 0 })
                {
                    var countries = await unitOfWork.Repository<Country>()
                        .Entities
                        .Where(p => request.FilmingCountryIds.Any(id => id == p.Id))
                        .ToListAsync(cancellationToken);
                    specification.FilmingCountries = countries;
                }

                await unitOfWork.Repository<FilmSpecification>().AddAsync(specification);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(specification.Id, localizer["Specification updated"]);
            }
            else
            {
                var dbSpecification = await unitOfWork.
                    Repository<FilmSpecification>().Entities
                    .Where(p=>p.Id==request.Id)
                    .Include(p=>p.FilmingCountries)
                    .FirstOrDefaultAsync(cancellationToken);
                if (dbSpecification != null)
                {
                    var updatedSpecification = mapper.Map(request, dbSpecification);
                    await UpdatedSubProjectTypes(request.SubProjectTypeIds, request.Id);
                   
                    updatedSpecification.FilmingCountries.Clear();

                    if (request.FilmingCountryIds is { Count: > 0 })
                    {
                        var countries = await unitOfWork.Repository<Country>()
                            .Entities
                            .Where(p => request.FilmingCountryIds.Any(id => id == p.Id))
                            .ToListAsync(cancellationToken);
                        updatedSpecification.FilmingCountries = countries;      
                    }
                  
                    
                    await unitOfWork.Repository<FilmSpecification>().UpdateAsync(updatedSpecification);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    return await Result<int>.SuccessAsync(dbSpecification.Id, localizer["Specification updated"]);
                }
                else
                {
                    return await Result<int>.FailAsync(localizer["Specification not found"]);
                }
            }
        }

        private async Task UpdatedSubProjectTypes(List<int> subProjectTypesId, int specificationId)
        {
            var dbspecificationProjectTypes = unitOfWork.Repository<SubProjectTypeFilmSpecification>().Entities
                .Where(p => p.FilmSpecificationId == specificationId);

            var deletedSpecificationProjectTypes = dbspecificationProjectTypes.Where(deadlneCat => !subProjectTypesId.Any(id => id == deadlneCat.Id))
               .ToList();

            var addedSpecificationProjectType = subProjectTypesId.Where(id => !dbspecificationProjectTypes.Any(focus => focus.Id == id))
                .ToList();

            if (deletedSpecificationProjectTypes != null)
            {
                foreach (var item in deletedSpecificationProjectTypes)
                {
                    await unitOfWork.Repository<SubProjectTypeFilmSpecification>().DeleteAsync(item);
                }
            }
            if (addedSpecificationProjectType != null)
            {
                foreach (var item in addedSpecificationProjectType)
                {
                    await unitOfWork.Repository<SubProjectTypeFilmSpecification>().AddAsync(new SubProjectTypeFilmSpecification()
                    {
                        FilmSpecificationId = specificationId,
                        SubProjectTypeId = item
                    });
                }
            }
        }
    }
}
