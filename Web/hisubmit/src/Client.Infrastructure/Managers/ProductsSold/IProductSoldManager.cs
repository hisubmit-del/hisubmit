using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Commands;

namespace HiSubmit.Client.Infrastructure.Managers.ProductsSold;

public interface IProductSoldManager:ITransientManager
{
    Task<PaginatedResult<GetAllSoldProductResponse>> GetAllAsync(GetAllSoldProductQuery query);
    Task<IResult<GetSoldProductDetailResponse>> GetById(GetSoldProductDetailQuery query);
    Task<IResult> AddAsync(AddProductSoldCommand command);
}

public class ProductSoldManager : IProductSoldManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public ProductSoldManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/productSold");
    }
    public async Task<PaginatedResult<GetAllSoldProductResponse>> GetAllAsync(GetAllSoldProductQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("getAll", query));
        return await response.ToPaginatedResult<GetAllSoldProductResponse>();
    }

    public async Task<IResult<GetSoldProductDetailResponse>> GetById(GetSoldProductDetailQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("detail", query));
        return await response.ToResult<GetSoldProductDetailResponse>();
    }

    public async Task<IResult> AddAsync(AddProductSoldCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Add"), command);
        return await response.ToResult();
    }
}