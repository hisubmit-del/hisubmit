using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using Hisubmit.Hisubmit.Client.SharedModels.Features.MasterFestivals.Queries;
using MediatR;

namespace HiSubmit.Application.Features.MasterFestivals.Queries;

public class GetAllMasterFestivalQuery:GetAllMasterFestivalRequest,IRequest<PaginatedResult<GetAllMasterFestivalResponse>>;

public class GetAllMasterFestivalQueryHandler(IMapper mapper,IUnitOfWork<int> unitOfWork)
    :IRequestHandler<GetAllMasterFestivalQuery,PaginatedResult<GetAllMasterFestivalResponse>>
{
    public async Task<PaginatedResult<GetAllMasterFestivalResponse>> Handle(GetAllMasterFestivalQuery request, CancellationToken cancellationToken)
    {
        var response = await unitOfWork.Repository<FestivalMaster>()
            .Entities
            .ProjectTo<GetAllMasterFestivalResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return response;
    }
}