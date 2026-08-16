using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Client.SharedModels.Features.Products.Commands.Enable;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;

namespace HiSubmit.Client.Infrastructure.Managers.AdminProducts;

public interface IAdminProductManager:ITransientManager
{
    Task<PaginatedResult<GetAllPagedProductsResponse>> GetAll(GetAllProductsRequest request);
    Task<IResult> UpdateEnable(EnableProductCommand command);
}

public class AdminProductManager(HttpClient httpClient) : IAdminProductManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/admin/product");

    public async Task<PaginatedResult<GetAllPagedProductsResponse>> GetAll(GetAllProductsRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("getAll", request));
        return await response.ToPaginatedResult<GetAllPagedProductsResponse>();
    }

    public async Task<IResult> UpdateEnable(EnableProductCommand command)
    {
        var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("updateEnable"), command);
        return await response.ToResult();
    }
}

