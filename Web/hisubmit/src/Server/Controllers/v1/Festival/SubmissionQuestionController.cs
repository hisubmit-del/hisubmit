using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.AddEdit;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Commands.Delete;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetAll;
using HiSubmit.Application.Features.Submission.SubmissionQuestions.Query.GetDetail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Festival
{
    public class SubmissionQuestionController : BaseFestivalController<SubmissionQuestionController>
    {
        /// <summary>
        /// Get All Submission form question for festival 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllSubmissionQuestionQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Get Detail Of Submission Question 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("Detail")]
        public async Task<IActionResult> GetDetail([FromQuery]GetSubmissionQuestionDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// Add or edit Question
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateQuestion(AddEditSubmissionQuestionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete question and option and category 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery]DeleteSubmissionQuestionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
