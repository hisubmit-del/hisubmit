using System.Threading.Tasks;
using HiSubmit.Application.Features.Tickets.Queries.GetAllTicket;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Public;

public class TicketController:BasePublicController<TicketController>
{
    /// <summary>
    /// Get All ProductFestivalId Ticket
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllTicketQuery query)
    {
        query.IsEnable = true;
        query.GetActiveTicket = true;
        return Ok(await Mediator.Send(query));
    }
}