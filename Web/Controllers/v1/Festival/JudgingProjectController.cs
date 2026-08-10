using HiSubmit.Application.Features.ProjectJudgings.Commands;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> AddRefereeToProject(AddEditProjectJudgingCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// get all project judging filter with user or submit id or festival
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("ProjecJudgings")]
        public async Task<IActionResult> GetAllProjectJudgings([FromQuery]GetAllProjectJudgingQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
