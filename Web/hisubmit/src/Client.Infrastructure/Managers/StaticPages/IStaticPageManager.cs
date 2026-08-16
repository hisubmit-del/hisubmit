using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.StaticPages;
public interface IStaticPageManager:ITransientManager
{
    Task<IResult> SaveAsync(AddEditStaticPageRequest request);
    Task<IResult> DeleteAsync(DeleteStaticPageCommand command);
    Task<PaginatedResult<GetAllStaticPageResponse>> GetAllAsync(GetAllStaticPageRequest request);
    Task<IResult<GetDetailStaticPageResponse>> GetDetailAsync(GetDetailStaticPageQuery query);
}

public  class  StaticPageManager:IStaticPageManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public StaticPageManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/staticPage");
    }
    public async Task<IResult> SaveAsync(AddEditStaticPageRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("save"), request);
        return await response.ToResult();
;    }

    public async Task<IResult> DeleteAsync(DeleteStaticPageCommand command)
    {
        var response = await _httpClient.DeleteAsync(_endPoint.GenerateUrl("delete",command));
        return await response.ToResult();
    }


    public async Task<PaginatedResult<GetAllStaticPageResponse>> GetAllAsync(GetAllStaticPageRequest request)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("getAll",request));
        return await response.ToPaginatedResult<GetAllStaticPageResponse>();
    }

    public async Task<IResult<GetDetailStaticPageResponse>> GetDetailAsync(GetDetailStaticPageQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("detail",query));
        return await response.ToResult<GetDetailStaticPageResponse>();
    }
}
