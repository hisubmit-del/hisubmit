using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Specifications.Festivals;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Festivals.Queries.GetAllOrginizer;

public class GetAllOrganizerQuery:IRequest<Result<List<GetAllEventOrganizerResponse>>>
{
    public int FestivalId { get; set; }
}
public class EventOrganizerQueryHandler(IMapper mapper, IAppCache appCache, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetAllOrganizerQuery, Result<List<GetAllEventOrganizerResponse>>>
{
    private readonly IAppCache _appCache = appCache;

    public async Task<Result<List<GetAllEventOrganizerResponse>>> Handle(GetAllOrganizerQuery request, CancellationToken cancellationToken)
    {
        var specification = new GetAllFestivalOrganizerSpecification(request);
        var getAllQuery = unitOfWork
            .Repository<EventOrginizer>()
            .Entities.Specify(specification)
            .ProjectTo<GetAllEventOrganizerResponse>(mapper.ConfigurationProvider);

        return await Result<List<GetAllEventOrganizerResponse>>
            .SuccessAsync(await getAllQuery.ToListAsync(cancellationToken));
          
    }
}