using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Catalog;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Hisubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Application.Features.Products.Queries.GetAllPaged;

public class GetAllProductsQuery : PagedRequest, IRequest<PaginatedResult<GetAllPagedProductsResponse>>
{
    public int? FestivalId { get; set; }
    //public string SearchString { get; set; }
    public bool? IsEnable { get; set; }
    //public string[] OrderBy { get; set; } // of the form fieldname [ascending|descending],fieldname [ascending|descending]...

    public GetAllProductsQuery()
    {

    }
    public GetAllProductsQuery(int pageNumber, int pageSize, string searchString, string orderBy, int? festivalId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        SearchString = searchString;
        FestivalId = festivalId;
        //if (!string.IsNullOrWhiteSpace(orderBy))
        //{
        //    OrderBy = orderBy.Split(',');
        //}
    }
}

internal class GetAllProductsQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper)
    : IRequestHandler<GetAllProductsQuery, PaginatedResult<GetAllPagedProductsResponse>>
{
    public async Task<PaginatedResult<GetAllPagedProductsResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var productFilterSpec = new ProductFilterSpecification(request.SearchString, request.FestivalId, request.IsEnable);
        //if (request.OrderBy?.Any() != true)
        //{
        //    var data = await unitOfWork.Repository<Product>()
        //        .Entities
        //        .Specify(productFilterSpec)
        //        .ProjectTo<GetAllPagedProductsResponse>(mapper.ConfigurationProvider)
        //        .ToPaginatedListAsync(request);
        //    return data;
        //}
        //else
        //{
        //   var ordering = string.Join(",", request.OrderBy); // of the form fieldname [ascending|descending], ...
        var data = await unitOfWork.Repository<Product>().Entities
            .Specify(productFilterSpec)
            //   .OrderBy(ordering) // require system.linq.dynamic.core
            .ProjectTo<GetAllPagedProductsResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return data;

        //}
    }
}