using HiSubmit.Application.Features.ProjectJudgings.Commands.AddProjectJudgingResult;
using HiSubmit.Application.Features.ProjectJudgings.Queries.CheckPermissionForJudging;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll;
using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetRefereeData;

namespace Web.Controllers.v1.Referees;

public class RefereeController : BaseApiController<RefereeController>
{
    private readonly ICurrentUserService _currentUserService;
    public RefereeController(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// get all user assigned refree to project
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllProjectJudgingQuery query)
    {
        // var userId = _currentUserService.UserId;
        // request.UserId = userId;
        query.FestivalId = null;
        query.SubmitId = null;

        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Checking the access level of the current user to judge a work
    /// </summary>
    /// <param name="projectUrl"></param>
    /// <returns></returns>
    [HttpGet("checkPermission/{projectUrl}")]
    public async Task<IActionResult> CheckPermissions(string projectUrl)
    {
        return Ok(await Mediator.Send(new CheckPermissionForJudgingQuery(projectUrl)));
    }


    /// <summary>
    /// Add judgment form data for submited project 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("AddJudgment")]
    public async Task<IActionResult> AddJudgment(AddEditProjectJudgingResultCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Get project judging Detail (referee name and id project detail and rating detail and answe question)
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("DetailJudging")]
    public async Task<IActionResult> GetJudgingDetail([FromQuery]GetProjectJudgingDetailQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get Referee Data such as count and average rate
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetUserRefereeData")]
    public async Task<IActionResult> GetUserRefereeData([FromQuery] GetRefereeDataQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}