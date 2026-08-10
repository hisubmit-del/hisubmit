using System.Threading.Tasks;
using HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Application.Features.Festivals.Queries.GetAllShowHall;
using HiSubmit.Application.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Application.Features.Festivals.Queries.GetVenueById;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Festival;

public class VenueController : BaseApiController<VenueController>
{
    /// <summary>
    /// Add or edit show hall with show times
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("saveShowHall")]
    public async Task<IActionResult> SaveShowHall(AddEditShowHallCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Get all venue from festival
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllVenueQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    
    /// <summary>
    /// Get All Show All For venue
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAllShowHall")]
    public async Task<IActionResult> GetAllShowHall([FromQuery]GetAllShowHallQuery query )
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// get venue detail include show hall and show times
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("VenueDetail")]
    public async Task<IActionResult> GetVenueDetail([FromQuery] GetVenueByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}