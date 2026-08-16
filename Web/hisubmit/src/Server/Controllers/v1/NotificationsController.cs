using System.Linq;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Notifications.Commands;
using HiSubmit.Application.Features.Notifications.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1;

public class NotificationsController : BaseApiController<NotificationsController>
{
    /// <summary>
    /// Get All User Notification
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("user")]
    public async Task<IActionResult> GetUserNotification([FromQuery]GetAllNotificationQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get All Admin Notification
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("admin")]
    public async Task<IActionResult> GetAdminNotifications([FromQuery]GetAllNotificationQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    
    /// <summary>
    /// Get ProductFestivalId Notification
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("festival")]
    public async Task<IActionResult> GetFestivalNotification([FromQuery] GetAllNotificationQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    
    /// <summary>
    /// seen notification 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("seen")]
    public async Task<IActionResult> SeenNotification(SeenNotificationCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}