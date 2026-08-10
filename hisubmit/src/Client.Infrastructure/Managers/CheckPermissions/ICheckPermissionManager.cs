using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Permissions.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
namespace HiSubmit.Client.Infrastructure.Managers.CheckPermissions;

public interface ICheckPermissionManager:ITransientManager
{
    Task<IResult<ProjectPermissionResponse>> CheckPermissionProject(CheckProjectPermissionQuery query);
}

public  class  CheckPermissionManager:ICheckPermissionManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public CheckPermissionManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/checkPermission");
    }
    public async Task<IResult<ProjectPermissionResponse>> CheckPermissionProject(CheckProjectPermissionQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("checkProjectPermission", query));
        return await response.ToResult<ProjectPermissionResponse>();
    }
}