using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalProductsSold;

public interface IFestivalSoldProductManager:ITransientManager
{
    Task<PaginatedResult<GetAllSoldProductResponse>> GetAllAsync(GetAllSoldProductQuery query);
    Task<IResult<GetSoldProductDetailResponse>> GetById(GetSoldProductDetailQuery query);
}

public class FestivalSoldProductManager : IFestivalSoldProductManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public FestivalSoldProductManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/productSold");
    }
    public async Task<PaginatedResult<GetAllSoldProductResponse>> GetAllAsync(GetAllSoldProductQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{query.FestivalId}/getAll"));
        return await response.ToPaginatedResult<GetAllSoldProductResponse>();
    }

    public async Task<IResult<GetSoldProductDetailResponse>> GetById(GetSoldProductDetailQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{query.FestivalId}/detail",query));
        return await response.ToResult<GetSoldProductDetailResponse>();
    }
}