using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Application.Features.Payments.DiscountsCodes.Queries;

public class GetAllDiscountCodeQuery:DiscountCodeFilter,IRequest<PaginatedResult<GetAllDiscountCodeResponse>>;

internal class GetAllDiscountCodeQueryHandler(IUnitOfWork<int> unitOfWork,IMapper mapper):IRequestHandler<GetAllDiscountCodeQuery,PaginatedResult<GetAllDiscountCodeResponse>>
{
    public async Task<PaginatedResult<GetAllDiscountCodeResponse>> Handle(GetAllDiscountCodeQuery request, CancellationToken cancellationToken)
    {
        var res = await unitOfWork.Repository<DiscountCode>()
            .Entities
            .Specify(new DiscountCodeFilterSpecification(request))
            .ProjectTo<GetAllDiscountCodeResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        return res;
    }
}