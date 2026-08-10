using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Submits;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities.Projects;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLineEventCategory;

public class GetAllDeadLineEventCategoryQuery : IRequest<Result<List<GetAllDeadLineEventCategoryResponse>>>
{
    public int? DeadLineId { get; set; }
    public int FestivalId { get; set; }
    public bool TakeCurrentDeadLine { get; set; }
    public bool SpecfyWithProject { get; set; }

    public int? ProjectId { get; set; }
    public bool? Nearest { get; set; }
}

public class GetAllDeadLineEventCategoryQueryHandler : IRequestHandler<GetAllDeadLineEventCategoryQuery,
    Result<List<GetAllDeadLineEventCategoryResponse>>>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetAllDeadLineEventCategoryQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<GetAllDeadLineEventCategoryResponse>>> Handle(
        GetAllDeadLineEventCategoryQuery request, CancellationToken cancellationToken)
    {
        var deadLineId = request.DeadLineId;
       
    

        IQueryable<DeadlineEventCategory> query;
        if (deadLineId == 0 || deadLineId == null)
        {
            query = _unitOfWork.Repository<DeadlineEventCategory>()
                .Entities
                .Include(p => p.DeadLine)
                .Include(p => p.EventCategory)
                .Where(p => p.EventCategory.FestivalId == request.FestivalId);
        }
        else
        {
            if (request.TakeCurrentDeadLine)
            {
                deadLineId = await _unitOfWork.Repository<DeadLine>()
                    .Entities
                    .Where(p => p.Date >= DateTime.UtcNow)
                    .OrderBy(p => p.Date)
                    .Select(p => p.Id)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            query = _unitOfWork.Repository<DeadlineEventCategory>()
                .Entities
                .Include(p => p.DeadLine)
                .Include(p => p.EventCategory)
                .Where(p => p.DeadLineId == deadLineId);
        }
        Project project=null;
        if (request.SpecfyWithProject && request.ProjectId != null)
        {
             project = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(request.ProjectId.Value);
             var countryId = await GetProjectCountry(project);
            var filterSpecification =
                new GetAllDeadLineEventCategoryFilter
                    (project.Size, project.StudentProject, project.ProjectType,countryId);
            
            query = query.Specify(filterSpecification);
        }

        var deadLineCategories = await query
            .ProjectTo<GetAllDeadLineEventCategoryResponse>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        if (request.Nearest == true)
        {
            deadLineCategories = await SelectNearestDeadLineCategory(deadLineCategories);
        }

        if (request.SpecfyWithProject && project !=null)
        {
            var festival = await _unitOfWork.Repository<Festival>()
                .GetByIdAsync(request.FestivalId);
            foreach (var dc in deadLineCategories)
            {
                dc.SelectedFeeType = GetMinPrice(dc, project.StudentProject, festival.FeeStatus);
            }
        }
        return await Result<List<GetAllDeadLineEventCategoryResponse>>
            .SuccessAsync(deadLineCategories);
    }

    private async Task<int?> GetProjectCountry(Project project)
    {
        int? countryId=0;
        switch (project.ProjectType)
        {
            case ProjectType.Film:
                 countryId = await _unitOfWork.Repository<FilmSpecification>()
                    .Entities.Where(p => p.ProjectId == project.Id)
                     .Select(p => p.OriginCountry.Id)
                    .FirstOrDefaultAsync();
                break;
            case ProjectType.Photography:
                countryId = await _unitOfWork.Repository<PhotographySpecification>()
                    .Entities.Where(p => p.ProjectId == project.Id)
                    .Select(p => p.OriginCountry.Id)
                    .FirstOrDefaultAsync();
                break;
            case ProjectType.Music:
                countryId = await _unitOfWork.Repository<MusicSpecification>()
                    .Entities.Where(p => p.ProjectId == project.Id)
                    .Select(p => p.OriginCountry.Id)
                    .FirstOrDefaultAsync();
                break;
            case ProjectType.Script_ScreenWriting:
                countryId = await _unitOfWork.Repository<ScriptSpecification>()
                    .Entities.Where(p => p.ProjectId == project.Id)
                    .Select(p => p.OriginCountry.Id)
                    .FirstOrDefaultAsync();
                break;
            case ProjectType.VR_XR:
                countryId = await _unitOfWork.Repository<XrVrSpecification>()
                    .Entities.Where(p => p.ProjectId == project.Id)
                    .Select(p => p.OriginCountry.Id)
                    .FirstOrDefaultAsync();
                break;
            case ProjectType.Art:
                countryId = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return countryId;
    }

    private async Task<List<GetAllDeadLineEventCategoryResponse>> SelectNearestDeadLineCategory(
        IEnumerable<GetAllDeadLineEventCategoryResponse> deadLineCategory)
    {
        var grouped = deadLineCategory
            .GroupBy(p => p.EventCategoryId)
            .ToList();

        return grouped.Select(deadCatGroup => deadCatGroup
                .Where(p => p.DeadLineDate.Date >= DateTime.Now.Date)
                .MinBy(p => p.DeadLineDate))
            .Where(nearDeadLine => nearDeadLine != null)
            .ToList();
    }
    private  FeeType GetMinPrice(GetAllDeadLineEventCategoryResponse deadLineCategory,
            bool studentProject,FeeStatus feeStatus)
    {
        var prices = new Dictionary<FeeType, int?> ();

        if(deadLineCategory.StandardFee !=null)
            prices.Add(FeeType.Standard, deadLineCategory.StandardFee);
        
        if (studentProject && deadLineCategory.StudentFee != null)
            prices.Add(FeeType.Student, deadLineCategory.StudentFee);

        if (feeStatus == FeeStatus.Special && deadLineCategory.GoldFee != null)
            prices.Add(FeeType.Gold, deadLineCategory.GoldFee);
        
        var f 
            = prices.FirstOrDefault(k => k.Value == prices.Values.Min());
        
        return f.Key;
    }
    
}