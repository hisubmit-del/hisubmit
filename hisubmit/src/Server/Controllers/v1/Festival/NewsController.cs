using System.Threading.Tasks;
using HiSubmit.Application.Features.News.Commands;
using HiSubmit.Application.Features.News.Queries;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class NewsController : BaseFestivalController<NewsController>
{
    /// <summary>
    /// get all new 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
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
    public async Task<IActionResult> Update(AddEditNewCommand command,int festivalId)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// delete new
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("delete")]
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
    public async Task<IActionResult> UpdateEnable(UpdateEnableNewCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}