using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetById;

namespace HiSubmit.Application.Features.Brands.Queries.GetById;

public class GetBrandByIdQuery :GetBrandByIdRequest, IRequest<Result<GetBrandByIdResponse>>;

internal class GetProductByIdQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    : IRequestHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
{
    public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
    {
        var brand = await unitOfWork.Repository<ArtCategory>().GetByIdAsync(query.Id);
        var mappedBrand = mapper.Map<GetBrandByIdResponse>(brand);
        return await Result<GetBrandByIdResponse>.SuccessAsync(mappedBrand);
    }
}