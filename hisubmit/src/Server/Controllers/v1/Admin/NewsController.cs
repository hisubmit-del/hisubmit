using System.Threading.Tasks;
using HiSubmit.Application.Features.News.Commands;
using HiSubmit.Application.Features.News.Queries;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Admin;

public class NewsController : BaseAdminController<NewsController>
{
    /// <summary>
    /// get all new 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    [Authorize(Permissions.Contents.View)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllNewQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// get new detail
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [Authorize(Permissions.Contents.View)]
    public async Task<IActionResult> GetDetail([FromQuery] GetDetailNewQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// add or edit new detail information
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Save")]
    [Authorize(Permissions.Contents.UpdateNew)]
    public async Task<IActionResult> Update(AddEditNewCommand command)
    {
        command.FestivalId = null;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// delete new
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("delete")]
    [Authorize(Permissions.Contents.UpdateNew)]
    public async Task<IActionResult> Delete([FromQuery] DeleteNewCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Update Enable (enable or disable new)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("enable")]
    [Authorize(Permissions.Contents.UpdateNew)]
    public async Task<IActionResult> UpdateEnable(UpdateEnableNewCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}