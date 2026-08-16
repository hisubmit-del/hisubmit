using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Festivals.Queries.GetFestivalNames;

public class GetFestivalNamesQuery :PagedRequest, IRequest<PaginatedResult<GetFestivalNamesResponse>>
{
       
    public  string FestivalIdString { get; set; }
    
    public List<int> GetFestivalId()
    {
        if (string.IsNullOrWhiteSpace(FestivalIdString))
            return new();
        return FestivalIdString.Split(',').Select(int.Parse).ToList();
    }

}
    
public class GetFestivalNamesQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper) :
    IRequestHandler<GetFestivalNamesQuery, PaginatedResult<GetFestivalNamesResponse>>
{
    public async Task<PaginatedResult<GetFestivalNamesResponse>> Handle(GetFestivalNamesQuery request, CancellationToken cancellationToken)
    {
        var festivals = await unitOfWork.Repository<Festival>()
            .Entities.Where(p => request.GetFestivalId().Any(id => id == p.Id))
            .ProjectTo<GetFestivalNamesResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        return festivals;
    }
}
