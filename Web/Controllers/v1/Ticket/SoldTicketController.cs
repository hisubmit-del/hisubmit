using System.Threading.Tasks;
using HiSubmit.Application.Features.SoldTickets.Commands;
using HiSubmit.Application.Features.SoldTickets.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.v1.Ticket;

public class SoldTicketController : BaseApiController<SoldTicketController>
{
    /// <summary>
    /// Add Ticket to sold card soldTicket status set to PaidAwaiting
    /// After user paid userCard set to paid and finish sale ticket 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("AddTicketToCart")]
    [Authorize]
    public async Task<IActionResult> AddToCard(AddSoldTicketCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    
    
    /// <summary>
    /// Add badge To user cart soldTicket status set to awaiting payment  
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("AddBadgeToCart")]
    [Authorize]
    public async Task<IActionResult> AddToCard(AddSoldBadgeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    
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

    [HttpGet("Download")]
    public async Task<IActionResult> DownloadFile([FromQuery]DownloadTicketsFileQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
