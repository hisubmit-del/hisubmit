using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace  HiSubmit.Client.Infrastructure.Managers.AdminNews;

public interface IAdminNewManager:ITransientManager
{
    Task<IResult> SaveAsync(AddEditNewCommand command);
    Task<IResult> DeleteAsync(DeleteNewCommand command);
    Task<IResult> UpdateEnableAsync(UpdateEnableNewCommand command);
    Task<PaginatedResult<GetAllNewResponse>> GetAllAsync(GetAllNewRequest request);
    Task<IResult<GetDetailNewResponse>> GetDetailAsync(GetDetailNewQuery query);
}

public  class  AdminNewManager:IAdminNewManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public AdminNewManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/news");
    }
    public async Task<IResult> SaveAsync(AddEditNewCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("save"), command);
        return await response.ToResult();
;    }

    public async Task<IResult> DeleteAsync(DeleteNewCommand command)
    {
        var response = await _httpClient.DeleteAsync(_endPoint.GenerateUrl("delete",command));
        return await response.ToResult();
    }

    public  async Task<IResult> UpdateEnableAsync(UpdateEnableNewCommand command)
    {
        var response = await _httpClient.PutAsJsonAsync(_endPoint.GenerateUrl("enable"), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllNewResponse>> GetAllAsync(GetAllNewRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("getAll",request));
        return await response.ToPaginatedResult<GetAllNewResponse>();
    }
    public async Task<IResult<GetDetailNewResponse>> GetDetailAsync(GetDetailNewQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("detail",query));
        return await response.ToResult<GetDetailNewResponse>();
    }
}

