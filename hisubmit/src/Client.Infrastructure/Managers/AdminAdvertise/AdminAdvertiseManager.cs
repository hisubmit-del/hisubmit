using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;

public interface IAdminAdvertiseManager:ITransientManager
{
    Task<IResult> DeleteBanner(DeleteAdvertiseBannerRequest cpRequest);
    Task<IResult> AddAdvertiseBanner(AddEditAdvertiseBannerRequest bannerRequest);
    Task<PaginatedResult<GetAllAdvertiseResponse>> GetAllAsync(GetAllAdvertiseRequest request);
    Task<IResult<GetDetailAdvertiseResponse>> GetDetailAsync(GetDetailAdvertiseRequest request);
    Task<PaginatedResult<GetAllAdvertiseBannerResponse>> GetAllBanner(GetAllAdvertiseBannerRequest request);
}

public class AdminAdvertiseManager:IAdminAdvertiseManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public AdminAdvertiseManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/advertises");
    }
    public async Task<PaginatedResult<GetAllAdvertiseResponse>> GetAllAsync(GetAllAdvertiseRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("GetAll",request));
        return await response.ToPaginatedResult<GetAllAdvertiseResponse>();
    }

    public async Task<IResult<GetDetailAdvertiseResponse>> GetDetailAsync(GetDetailAdvertiseRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("Detail",request));
        return await response.ToResult<GetDetailAdvertiseResponse>();
    }

    public async Task<IResult> DeleteBanner(DeleteAdvertiseBannerRequest cpRequest)
    {
        var response = await _httpClient.DeleteAsync(_endPoint.GenerateUrl("DeleteBanner", cpRequest));
        return await response.ToResult();
    }

    public async Task<IResult> AddAdvertiseBanner(AddEditAdvertiseBannerRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("AddBanner"), request);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllAdvertiseBannerResponse>> GetAllBanner(GetAllAdvertiseBannerRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("AllBanner",request));
        return await response.ToPaginatedResult<GetAllAdvertiseBannerResponse>();
    }
}

