using HiSubmit.Application.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using HiSubmit.Application.Features.FestivalFocs.Commands.DeleteFestivalFocus;
using HiSubmit.Application.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using HiSubmit.Application.Features.FestivalFocs.Queries.GetFestivalFocusDetail;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalContact;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.v1.Catalog
{
    public class FestivalFocusController : BaseApiController<FestivalFocusController>
    {
        /// <summary>
        /// Get All ProductFestivalId Focus
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("GetAll")]
        //[Authorize(Permissions.FocusCategory.View)]
        public async Task<IActionResult> GetAll([FromQuery]GetAllFestivalFocusQuery query)
        {
            return Ok (await Mediator.Send(query));
        }


        /// <summary>
        /// Get ProductFestivalId Detail By Id
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("Detail")]
        [Authorize(Permissions.FocusCategory.View)]
        public async Task<IActionResult> GetById([FromQuery]GetFestivalFocusDeailQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// add or edit Focus 
        /// </summary>
        /// <param name="command"></param>
        /// <returns>added or updated item id</returns>
        [HttpPost("Update")]
        [Authorize(Permissions.FocusCategory.Edit)]

        public async Task<IActionResult> Update(AddEditFestivalFocusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete ProductFestivalId Focus  By Id
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpDelete("Delete")]
        [Authorize(Permissions.FocusCategory.Edit)]
        public async Task<IActionResult> Delete([FromQuery]DeleteFestivalFocusCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
