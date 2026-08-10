using System.Threading.Tasks;
using HiSubmit.Application.Features.Permissions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.CheckPermissions;

public class CheckPermissionController : BaseApiController<CheckPermissionController>
{
    /// <summary>
    /// check permission for project
    /// in project page if user is artist 
    /// </summary>
    /// <param name="query">
    /// request.projectId :ای دی پروژه ای که قصد تعیین دسترسی را داریم 
    /// </param>
    /// <returns>
    /// if user is artist for project return permission.write
    /// if user is not artist for project return permission.read
    /// </returns>
    [HttpGet("checkProjectPermission")]
    public async Task<IActionResult> CheckProjectPermission([FromQuery]CheckProjectPermissionQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
