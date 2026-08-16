using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgiingButton;
using HiSubmit.Application.Features.Judgings.Commands.AddEditJudgingButton;
using HiSubmit.Application.Features.Judgings.Commands.DeleteJudgiingFiiled;
using HiSubmit.Application.Features.Judgings.Commands.DeleteJudgingButtons;
using HiSubmit.Application.Features.Judgings.Queries.Detail;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class JudgingController : BaseFestivalController<JudgingController>
{

    /// <summary>
    /// Get judging Form for each _project type
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("Detail")]
    public async Task<IActionResult> GetDetail([FromQuery] GetJudgingDetailQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// add or ediit judging form buttons
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("UpdateButton")]
    public async Task<IActionResult> AddEditButton( AddEditJudgingButtonCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// add or edit judging form fileds
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("UpdateFiled")]
    public async Task<IActionResult> AddEditFiled( AddEditJudgingFiledCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// delete judging form fiiled
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("DeleteFiled")]
    public async Task<IActionResult> DeleteFiled([FromQuery] DeleteJudgingFiledCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// delete judging form button 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpDelete("DeleteButton")]
    public async Task<IActionResult> DeleteButton([FromQuery] DeleteJudgingButtonCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}