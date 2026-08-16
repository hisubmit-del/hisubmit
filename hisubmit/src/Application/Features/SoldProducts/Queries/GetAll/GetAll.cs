using System;
using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Linq;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Base;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.SoldProducts;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.SoldProducts.Queries;

public class GetAllSoldProductQuery :
    PagedRequest, IRequest<PaginatedResult<GetAllSoldProductResponse>>
{
    public int? FestivalId { get; set; }
    public new string SearchString { get; set; }
    public RequestAccountType RequestAccountType { get; set; }
}

public class GetAllSoldProductQueryHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    ICurrentUserService currentUserService)
    :
        IRequestHandler<GetAllSoldProductQuery, PaginatedResult<GetAllSoldProductResponse>>
{
    public async Task<PaginatedResult<GetAllSoldProductResponse>> Handle
        (GetAllSoldProductQuery request, CancellationToken cancellationToken)
    {
        var specify = GenerateSpecify(request);
        var filter = new GetAllSoldProductFilterSpecification(request.SearchString);
        var resul = await unitOfWork.Repository<ProductSold>()
            .Entity
            .Include(p => p.Product)
            .Where(specify.Criteria)
            .Where(filter.Criteria)
            .ProjectTo<GetAllSoldProductResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return resul;
    }

    private HeroSpecification<ProductSold> GenerateSpecify(GetAllSoldProductQuery query)
    {
        switch (query.RequestAccountType)
        {
            case RequestAccountType.User:
                return new GetAllUserSoldProductSpecification(currentUserService.UserId);
            case RequestAccountType.Festival:
                return new GetAllFestivalSoldProductSpecification(query.FestivalId.Value);
            case RequestAccountType.Admin:
                return new GetAllAdminProductSoldSpecification();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
