using System.Threading.Tasks;
using HiSubmit.Application.Features.Tickets.Commands.AddEditTickets;
using HiSubmit.Application.Features.Tickets.Commands.DeleteTicket;
using HiSubmit.Application.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Application.Features.Tickets.Queries.GetTicketById;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class TicketController:BaseFestivalController<TicketController>
{

    /// <summary>
    /// Add Or Edit Ticket
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("save")]
    public async Task<IActionResult> SaveTicket(AddEditTicketsCommand command, int festivalId)
    {
        
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// get all ticket for festival 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllTicketQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// get ticket detail (showtime and ...)
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("Detail")]
    public async Task<IActionResult> GetDetail([FromQuery]GetTicketByIdQuery query,int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// delete ticket 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpDelete("Delete")]
    public async Task<IActionResult> Delete([FromQuery] DeleteTicketCommand command,int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }
}