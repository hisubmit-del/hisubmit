using HiSubmit.Application.Features.ProjectJudgings.Commands;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Web.Filters;

namespace Web.Controllers.v1.Festival
{
    public class JudgingProjectController:BaseFestivalController<JudgingProjectController>
    {
        /// <summary>
        /// Add Referee To project 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("AddRefree")]
        [FestivalAuthentication(Policy = Permissions.Submits.AddToReferee)]
        public async Task<IActionResult> AddRefereeToProject(
            AddEditProjectJudgingCommand command,
            int festivalId)
        {
            command = command with { FestivalId = festivalId };
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// get all project judging filter with user or submit id or festival
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("ProjecJudgings")]
        [FestivalAuthentication(Policy = Permissions.Judging.View)]
        public async Task<IActionResult> GetAllProjectJudgings(
            [FromQuery] GetAllProjectJudgingQuery query,
            int festivalId)
        {
            query.FestivalId = festivalId;
            return Ok(await Mediator.Send(query));
        }
    }
}
