using HiSubmit.Application.Features.Submits.Commands;
using HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Reviews.Commands;
using HiSubmit.Application.Features.Reviews.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.v1;

public class ProjectSubmittedController : BaseApiController<ProjectSubmittedController>
{

    /// <summary>
    /// AddSubmit to festival to specify selected category
    /// </summary>
    /// <param name="command"></param>
    /// <returns>submitted Id</returns>
    [HttpPost("Submit")]
    [Authorize]
    public async Task<IActionResult> SubmitToFestival(AddSubmitCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Return all submit with request filter (for user or project or festival and status )
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllSubmitsQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// final result of submitted by main festival user
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("FinalResult")]
    public async Task<IActionResult>FinalResult(AddEditFinalJudgingCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

        
    /// <summary>
    /// with draw project from festival(submit status is withdrawn and projectJudging inactivate )
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("WithDraw")]
    public async Task<IActionResult> WithDrawProject(WithDrawProjectCommand command)
    {
        return Ok(await  Mediator.Send(command));
    }

    /// <summary>
    /// Add Review For ProductFestivalId 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Review")]
    public async Task<IActionResult> Review(AddReviewCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// All Review 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllReview")]
    public async Task<IActionResult> GetAllReview([FromQuery]GetAllReviewQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
