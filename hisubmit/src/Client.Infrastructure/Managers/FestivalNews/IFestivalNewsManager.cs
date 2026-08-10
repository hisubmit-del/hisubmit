using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.FestivalNews;

public interface IFestivalNewsManager:ITransientManager
{
    Task<IResult> SaveAsync(AddEditNewCommand command,int festivalId);
    Task<IResult> DeleteAsync(DeleteNewCommand command,int festivalId);
    Task<IResult> UpdateEnableAsync(UpdateEnableNewCommand command,int festivalId);
    Task<PaginatedResult<GetAllNewResponse>> GetAllAsync(GetAllNewRequest request,int festivalId);
    Task<IResult<GetDetailNewResponse>> GetDetailAsync(GetDetailNewQuery query,int festivalId);
}

public class FestivalNewsManager:IFestivalNewsManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public FestivalNewsManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/news");
    }
    public async Task<IResult> SaveAsync(AddEditNewCommand command,int festivalId)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/save"), command);
        return await response.ToResult();
        ;    }

    public async Task<IResult> DeleteAsync(DeleteNewCommand command,int festivalId)
    {
        var response = await _httpClient.DeleteAsync(_endPoint.GenerateUrl($"{festivalId}/delete",command));
        return await response.ToResult();
    }

    public  async Task<IResult> UpdateEnableAsync(UpdateEnableNewCommand command,int festivalId)
    {
        var response = await _httpClient.PutAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/enable"), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllNewResponse>> GetAllAsync(GetAllNewRequest request,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/getAll",request));
        return await response.ToPaginatedResult<GetAllNewResponse>();
    }

    public async Task<IResult<GetDetailNewResponse>> GetDetailAsync(GetDetailNewQuery query,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/detail",query));
        return await response.ToResult<GetDetailNewResponse>();
    }
}