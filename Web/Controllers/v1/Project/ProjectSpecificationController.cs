using HiSubmit.Application.Features.Projects.Commands.AddEditFilmSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditMusicSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditPhotographySpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditScriptSpecification;
using HiSubmit.Application.Features.Projects.Commands.AddEditVrXrSpecification;
using HiSubmit.Application.Features.Projects.Queries.GetFilmSpecificationDetail;
using HiSubmit.Application.Features.Projects.Queries.GetMusicSpecificationDetail;
using HiSubmit.Application.Features.Projects.Queries.GetVrXrSpecificationDetail;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Web.Controllers.v1.Project
{
    public class ProjectSpecificationController : BaseApiController<ProjectSpecificationController>
    {
        /// <summary>
        /// update Film FilmSpecification Filed in project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateFilmSpecification")]
        [Authorize]
        public async Task<IActionResult> UpdateFilmSpecification(AddEditFilmSpecificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }


        /// <summary>
        /// get specification detail 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("FilmSpecificationDetail")]
        [Authorize]
        public async Task<IActionResult> GetFilmSpecification([FromQuery]GetFilmSpecificationDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// update Film FilmSpecification Filed in project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateMusicSpecification")]
        [Authorize]
        public async Task<IActionResult> UpdateMusicSpecification(AddEditMusicSpecificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// get specification detail 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("MusicSpecificationDetail")]
        [Authorize]
        public async Task<IActionResult> GetMusicSpecification([FromQuery]GetMusicSpecificationDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// update Film FilmSpecification Filed in project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateScriptSpecification")]
        [Authorize]
        public async Task<IActionResult> UpdateScriptSpecification(AddEditScriptSpecificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// get specification detail 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("ScriptSpecificationDetail")]
        [Authorize]
        public async Task<IActionResult> GetScriptSpecification([FromQuery] GetScriptSpecificationDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// update Film FilmSpecification Filed in project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdatePhotographySpecification")]
        [Authorize]
        public async Task<IActionResult> UpdatePhotographySpecification(AddEditPhotographySpecificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// get specification detail 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("PhotographySpecificationDetail")]
        [Authorize]
        public async Task<IActionResult> GetPhotographySpecification([FromQuery] GetPhotographySpecificationDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }


        /// <summary>
        /// update Film FilmSpecification Filed in project
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("UpdateVrXrSpecification")]
        [Authorize]
        public async Task<IActionResult> UpdateVrXrSpecification(AddEditVrXrSpecificationCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// get specification detail 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("VrXrSpecificationDetail")]
        [Authorize]
        public async Task<IActionResult> GetVrXrSpecification([FromQuery] GetVrXrSpecificationDetailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
