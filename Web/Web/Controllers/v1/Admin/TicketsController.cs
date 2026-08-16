using System.Threading.Tasks;
using HiSubmit.Application.Features.Tickets.Commands.Enable;
using HiSubmit.Application.Features.Tickets.Queries.GetAllTicket;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin;

public class TicketsController : BaseAdminController<TicketsController>
{
    /// <summary>
    /// get all ticket for festival 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllTicketQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    /// <summary>
    /// enable or disable festival  tickets 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("UpdateEnable")]
    public async Task<IActionResult> UpdateEnable(EnableTicketCommand request)
    {
        return  Ok(await  Mediator.Send(request));
    }
}