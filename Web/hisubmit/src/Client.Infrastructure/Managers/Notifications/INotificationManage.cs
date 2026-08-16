using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.Notifications.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Notifications;

public interface INotificationManager:ITransientManager
{
    Task<IResult> SeenNotifications(SeenNotificationCommand command);
    Task<PaginatedResult<GetAllNotificationResponse>> GetUserNotifications(GetAllNotificationQuery query);
    Task<PaginatedResult<GetAllNotificationResponse>> GetAdminNotifications(GetAllNotificationQuery query);
    Task<PaginatedResult<GetAllNotificationResponse>> GetFestivalNotifications(GetAllNotificationQuery query);

}

public class NotificationManager : INotificationManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public NotificationManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/Notifications");
    }

    public async Task<IResult> SeenNotifications(SeenNotificationCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseEndPoint.GenerateUrl("seen"), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllNotificationResponse>> GetUserNotifications(GetAllNotificationQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("user", query));
        return await response.ToPaginatedResult<GetAllNotificationResponse>();
    }

    public async Task<PaginatedResult<GetAllNotificationResponse>> GetAdminNotifications(GetAllNotificationQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("admin", query));
        return await response.ToPaginatedResult<GetAllNotificationResponse>();
    }

    public async Task<PaginatedResult<GetAllNotificationResponse>> GetFestivalNotifications(GetAllNotificationQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("festival", query));
        return await response.ToPaginatedResult<GetAllNotificationResponse>();
    }
}