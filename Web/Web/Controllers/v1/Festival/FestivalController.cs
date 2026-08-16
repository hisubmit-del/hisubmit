using HiSubmit.Application.Features.Festivals.Commands.AddEdiitEventOrginizer;
using HiSubmit.Application.Features.Festivals.Commands.AddEditAdditinalSettings;
using HiSubmit.Application.Features.Festivals.Commands.AddEditDeadLineEntry;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalContact;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalDeadlines;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalVenue;
using HiSubmit.Application.Features.Festivals.Commands.CreateFestival;
using HiSubmit.Application.Features.Festivals.Commands.DeleteDeadLineEntry;
using HiSubmit.Application.Features.Festivals.Commands.DeleteEventOrginizer;
using HiSubmit.Application.Features.Festivals.Commands.DeleteVenue;
using HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLine;
using HiSubmit.Application.Features.Festivals.Queries.GetAllOrginizer;
using HiSubmit.Application.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Application.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Application.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Application.Interfaces.Services;
using Web.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalImages;
using HiSubmit.Application.Features.Festivals.Commands.AddFestival;
using HiSubmit.Application.Features.Festivals.Commands.ReleaseFestival;
using HiSubmit.Application.Features.Festivals.Commands.SpecialRequest;
using HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalPeriods;
using HiSubmit.Application.Features.Festivals.Queries.GetAllImages;
using HiSubmit.Application.Features.Festivals.Queries.GetFestivalNames;
using HiSubmit.Application.Features.Reviews.Queries;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.v1.Festival;

public class FestivalController : BaseFestivalController<FestivalController>
{
    private readonly IUploadService _uploadService;
    private readonly ICurrentUserService _currentUserService;
    public FestivalController(IUploadService uploadService,ICurrentUserService currentUserService)
    {
        _uploadService = uploadService;
        _currentUserService = currentUserService;
    }

    [HttpPost("SpecialRequest")]
    public async Task<IActionResult> SpecialRequest(SpecialRequestCommand command,int FestivalId)
    {
        command.FestivalId = FestivalId;

        return Ok(await Mediator.Send(command));
    } 

    /// <summary>
    /// get festival names (just name)
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetFestivalNames")]
    public async Task<IActionResult> GetFestivalNames([FromQuery]GetFestivalNamesQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Release ProductFestivalId if required filed is full 
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Release")]
    public async Task<IActionResult> ReleaseFestival(ReleaseFestivalCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
        
        
    /// <summary>
    /// return festival detail
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("GetById")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]

    public async Task<IActionResult> GetById([FromQuery] GetFestivalDetailByIdQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }


    [HttpPost("AddFestival")]
    [Authorize]
    public async Task<IActionResult> AddFestival(AddFestivalCommand command)
    {
        if (!_currentUserService.IsAuthenticated)
            return Ok(new Result() { Succeeded = false, Messages = ["currer"] });
        command.AddToCurrentUser = true;
        return Ok(await Mediator.Send(command));
    }
        
    /// <summary>
    /// updated detail property of festival
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>updated festival id</returns>
    [HttpPost("UpdateDetail")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> UpdateDetail(AddEditFestivalDetailCommand command, int festivalId)
    {
        command.Id = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Updated contact property of festival
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>festival updated id</returns>
    [HttpPost("UpdateContact")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> UpdateContact(AddEditFestivalContactCommand command, int festivalId)
    {
        command.Id = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Add and edit orginizer item
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>updated or added item id</returns>
    [HttpPost("AddOrganizer")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> AddEditOrganizer(AddEditEventOrginizerCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// all organizer of festival
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns>list of organizer</returns>
    [HttpGet("GetAllOrganizer")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> GetAllOrganizer([FromQuery] GetAllOrganizerQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// delete organizer item with id
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>deleted organizer by id</returns>
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    [HttpDelete("DeleteOrganizer")]
    public async Task<IActionResult> DeleteOrganizer([FromQuery]DeleteEventOrginizerCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Add and edit venue updated
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>venue updated id</returns>
    [HttpPost("UpdateVenue")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult>UpdateVenue(AddEditFestivalVenueCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Get venue List of festival
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns>List of Venue Without include Address</returns>
    [HttpGet("GetAllVenue")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult>GetAllVenue([FromQuery]GetAllVenueQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get Venue Detail For edit and view and ...
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns>Venue Item With include Address</returns>
    [HttpGet("VenueDetail")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> GetVenueById([FromQuery]GetVenueByIdQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Delete ProductFestivalId Venue Item
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>Venue deleted Id</returns>
    [HttpDelete("DeleteVenue")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> DeleteVenue([FromQuery]DeleteVenueCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Update DeadLine Property in ProductFestivalId Tb
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>ProductFestivalId Updated Id</returns>
    [HttpPost("UpdateDeadline")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult>UpdateDeadLine(AddEditFestivalDeadlineCommand command, int festivalId)
    {
        command.Id = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Get DeadLine entry Detail For Edit and view and ...
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns>Detail DeadLine</returns>
    [HttpGet("DetailDeadLine")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> DetailDeadLine([FromQuery]GetDeadLineByIdQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get All DEadLineEntry
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns>list Of DeadLine </returns>
    [HttpGet("AllDeadLineEntry")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult>AllDeadLineEntry([FromQuery]GetAllDeadlineQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Add And Edit DeadLine entry Item
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>DeadLine Item Id</returns>
    [HttpPost("AddEditDeadLineEntry")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult>AddEditDeadLineEntry(AddEditDeadLineEntryCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Delete DeadLine Entry item
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns>Deleted Item Id</returns>
    [HttpDelete("DeleteDeadLine")]  
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult>DeleteDeadLine([FromQuery]DeleteDeadLineEntryCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

    /// <summary>
    /// Update Additional Setting Property
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("UpdateAdditionalSetting")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult>UpdateAdditionalSetting(AddEditAdditionalSettingCommand command, int festivalId)
    {
        command.Id = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Upload festival galleries
    /// </summary>
    /// <param name="command"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpPost("UploadImages")]
    [FestivalAuthentication(Policy = Permissions.Festival.Edit)]
    public async Task<IActionResult> UploadImages(AddEditFestivalImageCommand command,int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// get all festival gallleries image
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("Images")]
    [FestivalAuthentication(Policy = Permissions.Festival.View)]
    public async Task<IActionResult> GetAllImage([FromQuery] GetAllFestivalImageQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("AllPeriods")]
    public async Task<IActionResult> GetAllPeriods([FromQuery] GetAllFestivalPeriodsQuery query,int festivalId)
    {
        // request.ProductFestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("AllReview")]
    public async Task<IActionResult> GetAllReviews([FromQuery] GetAllReviewQuery query,int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }
}
