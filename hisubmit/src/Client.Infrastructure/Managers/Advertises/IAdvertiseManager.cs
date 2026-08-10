using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Advertises;

public interface IAdvertiseManager:ITransientManager
{
    Task<IResult> AddAdvertise(AddAdvertiseRequest request);
}

public class AdvertiseManager : IAdvertiseManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public AdvertiseManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/public/advertise");
    }
    public async Task<IResult> AddAdvertise(AddAdvertiseRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("addRequest"), request);
        return await response.ToResult();
    }
}
