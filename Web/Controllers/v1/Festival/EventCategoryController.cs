using HiSubmit.Application.Features.Festivals.Commands.AddEditEventCategory;
using HiSubmit.Application.Features.Festivals.Commands.DeleteEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetAllEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetEventCateoryById;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Web.Controllers.v1.Festival
{
    public class EventCategoryController : BaseApiController<EventCategoryController>
    {
        /// <summary>
        /// Get all Event Category for festival 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>list of event catefory without include deadLine</returns>
        [HttpGet("AllCategory")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllEventCategoryQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        /// <summary>
        /// Add or edit Ecent Category (Add just with Name and update with description and deadLine and ...)
        /// </summary>
        /// <param name="command"></param>
        /// <returns>updated or added item Id</returns>
        [HttpPost("UpdateCategory")]
        public async Task<IActionResult> UpdateCategory(AddEditEventCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Get EventCategory Detail By Id with include deadLine list 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Event Category object with details </returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById([FromQuery]GetEventCategoryByIdQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery]DeleteEventCategoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}

