using System.Threading.Tasks;
using HiSubmit.Application.Features.SoldTickets.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Festival;

public class SoldTicketController : BaseFestivalController<SoldTicketController>
{
    /// <summary>
    /// Get All Sold Ticket For User
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllSoldTicketQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
