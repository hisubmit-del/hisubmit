using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Submits.Queries.GetAllSubmitCategories;

public class GetAllSubmitCategoriesQuery :
    PagedRequest, IRequest<PaginatedResult<GetAllSubmitCategoriesResponse>>
{
    public int SubmitId { get; set; }
    public int FestivalId { get; set; }
    public  RequestSubmitCategoriesType Type { get; set; }
}

public enum RequestSubmitCategoriesType:int
{
    Submit=0,
    Festival=1
}
public class GetAllSubmitCategoryQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork) :
    IRequestHandler<GetAllSubmitCategoriesQuery, PaginatedResult<GetAllSubmitCategoriesResponse>>
{
    public async Task<PaginatedResult<GetAllSubmitCategoriesResponse>> Handle
        (GetAllSubmitCategoriesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<SubmitDeadLineCategories,bool>> expression;
        switch (request.Type)
        {
            case RequestSubmitCategoriesType.Submit:
                expression = p => p.SubmitId == request.SubmitId;
                break;
            case RequestSubmitCategoriesType.Festival:
                expression = p => p.Submit.FestivalId == request.FestivalId;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        var categoryDeadlines = await unitOfWork.Repository<SubmitDeadLineCategories>()
            .Entities
            .Where(expression)
            .Include(p=>p.DeadlineEventCategory).ThenInclude(p=>p.EventCategory)
            .Include(p=>p.DeadlineEventCategory).ThenInclude(p=>p.DeadLine)
            .ProjectTo<GetAllSubmitCategoriesResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        return categoryDeadlines;
    }
}