using System.Threading.Tasks;
using HiSubmit.Application.Features.News.Commands;
using HiSubmit.Application.Features.News.Queries;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Festival;

public class NewsController : BaseFestivalController<NewsController>
{
    /// <summary>
    /// get all new 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    [FestivalAuthentication(Policy = Permissions.FestivalNews.View)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllNewQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        query.GetFestivalNews = true;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// get new detail
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("detail")]
    [FestivalAuthentication(Policy = Permissions.FestivalNews.View)]
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
    [FestivalAuthentication(Policy = Permissions.FestivalNews.Edit)]
    public async Task<IActionResult> Update(AddEditNewCommand command,int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// delete new
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("delete")]
    [FestivalAuthentication(Policy = Permissions.FestivalNews.Edit)]
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
    [FestivalAuthentication(Policy = Permissions.FestivalNews.Edit)]
    public async Task<IActionResult> UpdateEnable(UpdateEnableNewCommand command)
    {
        command.FestivalId = RouteData.Values.TryGetValue("festivalId", out var value) &&
                             int.TryParse(value?.ToString(), out var festivalId)
            ? festivalId
            : command.FestivalId;
        return Ok(await Mediator.Send(command));
    }
}
