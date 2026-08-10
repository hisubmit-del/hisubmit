using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using Hisubmit.Client.SharedModels.Requests.Catalog;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;

namespace HiSubmit.Client.Infrastructure.Managers.Catalog.Product;

public class ProductManager(HttpClient httpClient) : IProductManager
{
    public async Task<IResult<int>> DeleteAsync(int id,int festivalId)
    {
        var response = await httpClient.DeleteAsync($"{Routes.ProductsEndpoints.Delete(festivalId)}/{id}");
        return await response.ToResult<int>();
    }

    public async Task<IResult<string>> ExportToExcelAsync(int festivalId,string searchString = "")
    {
        var response = await httpClient.GetAsync(string.IsNullOrWhiteSpace(searchString)
            ? Routes.ProductsEndpoints.Export(festivalId)
            : Routes.ProductsEndpoints.ExportFiltered(searchString,festivalId));
        return await response.ToResult<string>();
    }

    public async Task<IResult<AddEditProductRequest>> GetByIdAsync(GetProductByIdRequest request,int festivalId)
    {
        var response = await httpClient.GetAsync(Routes.ProductsEndpoints.Get(festivalId)+$"?id={request.Id}");
        return await response.ToResult<AddEditProductRequest>();
    }

    public async Task<IResult<string>> GetProductImageAsync(int id,int festivalId)
    {
        var response = await httpClient.GetAsync(Routes.ProductsEndpoints.GetProductImage(id,festivalId));
        return await response.ToResult<string>();
    }

    public async Task<PaginatedResult<GetAllPagedProductsResponse>> GetProductsAsync(GetAllPagedProductsRequest request)
    {
        var response = await httpClient.PostAsJsonAsync($"api/v1/products/{request.FestivalId}/getAll",request);
        return await response.ToPaginatedResult<GetAllPagedProductsResponse>();
    }

    public async Task<IResult<int>> SaveAsync(AddEditProductRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(Routes.ProductsEndpoints.Save(request.FestivalId), request);
        return await response.ToResult<int>();
    }
}