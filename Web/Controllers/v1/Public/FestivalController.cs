using HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLine;
using HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLineEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetAllOrginizer;
using HiSubmit.Application.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Application.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Application.Features.Festivals.Queries.GetVenueById;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Application.Features.Brands.Queries.GetAll;
using HiSubmit.Application.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using HiSubmit.Application.Features.FestivalLikes;
using HiSubmit.Application.Features.Festivals.Queries.GetAllImages;
using HiSubmit.Application.Features.News.Queries;
using HiSubmit.Application.Features.Products.Queries.GetAllPaged;
using HiSubmit.Application.Features.Reviews.Commands;
using HiSubmit.Application.Features.Reviews.Queries;
using HiSubmit.Application.Features.FestivalQualifyers.Queries.GetAll;
using HiSubmit.Application.Features.Products.Queries.GetById;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Microsoft.AspNetCore.Authorization;
using HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalFile;

namespace Web.Controllers.v1.Public;

public class FestivalController : BasePublicController<FestivalController>
{
    /// <summary>
    /// return get all enable festival
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpPost("GetAll")]
    public async Task<IActionResult> GetAll(GetAllFestivalQuery query)
    {
        query.IsActive = true;
        query.PublicOnly = true;
        query.IsActivePeriod = true;
        var res = await Mediator.Send(query);
        return Ok(res);
    }

    /// <summary>
    /// return festival detail
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetById")]
    public async Task<IActionResult> GetById([FromQuery] GetFestivalDetailByIdQuery query)
    {
        var res = await Mediator.Send(query);
        if (res.Succeeded && !res.Data.Public)
        {
            res.Data = new GetFestivalDetailResponse();
            res.Data.Public = false;
            return Ok(res);
        }
        return Ok(res);
    }


    /// <summary>
    /// all organizer of festival
    /// </summary>
    /// <param name="query"></param>
    /// <returns>list of organizer</returns>
    [HttpGet("GetAllOrganizer")]
    public async Task<IActionResult> GetAllOrganizer([FromQuery] GetAllOrganizerQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get venue List of festival
    /// </summary>
    /// <param name="query"></param>
    /// <returns>List of Venue Without include Address</returns>
    //[HttpGet("{festivalId:int}/GetAllVenue")]
    [HttpGet("GetAllVenue")]
    public async Task<IActionResult> GetAllVenue([FromQuery] GetAllVenueQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get Venue Detail For edit and view and ...
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Venue Item With include Address</returns>
    [HttpGet("VenueDetail")]
    public async Task<IActionResult> GetVenueById([FromQuery] GetVenueByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get DeadLine entry Detail For Edit and view and ...
    /// </summary>
    /// <param name="query"></param>
    /// <returns>Detail DeadLine</returns>
    [HttpGet("DetailDeadLine")]
    public async Task<IActionResult> DetailDeadLine([FromQuery] GetDeadLineByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get All DEadLineEntry
    /// </summary>
    /// <param name="query"></param>
    /// <returns>list Of Dead Line </returns>
    [HttpGet("AllDeadLineEntry")]
    public async Task<IActionResult> AllDeadLineEntry([FromQuery] GetAllDeadlineQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// Get all deadline event category with special id or next deadline (next deadline computed in request handler)
    /// </summary>
    /// <param name="query"></param>
    /// <returns>list Of  event category </returns>
    [HttpGet("AllDeadlineEventCategory")]
    public async Task<IActionResult> AllDeadLineEventCategory([FromQuery] GetAllDeadLineEventCategoryQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// get all festival gallleries image
    /// </summary>
    /// <param name="query"></param>
    /// <param name="festivalId"></param>
    /// <returns></returns>
    [HttpGet("Images")]
    public async Task<IActionResult> GetAllImage([FromQuery] GetAllFestivalImageQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get All ProductFestivalId Reviews
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllReviews")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllReviewQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// get all new 
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("GetAllNews")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllNewQuery query)
    {
        query.IsEnable = true;
        // This endpoint is used by a festival detail page. When a festival
        // id is supplied, do not leak news belonging to other festivals.
        query.GetFestivalNews = query.FestivalId.HasValue;
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// Get All ProductFestivalId Produts
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllProducts")]
    public async Task<IActionResult> GetAllProduct([FromQuery] GetAllProductsQuery query)
    {
        query.IsEnable = true;
        return Ok(await Mediator.Send(query));
    }

    
    /// <summary>
    /// Get All Art Category
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllArtCategory")]
    public async Task<IActionResult> GetAllEventCategory([FromQuery] GetAllArtCategoryQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    
    /// <summary>
    /// get all festival focus
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("AllFestivalFocus")]
    public async Task<IActionResult> GetAllFestivalFocus([FromQuery] GetAllFestivalFocusQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    
    /// <summary>
    /// Review and comment for user that not submitted to festival
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("Review")]
    [Authorize]
    public async Task<IActionResult> Review(AddReviewCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    /// <summary>
    /// Get ProductFestivalId Qualifires
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("Qualifiers")]
    public async Task<IActionResult> Qualifires
        ([FromQuery]GetAllFestivalQualifiersQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("Product")]
    public async Task<IActionResult> GetProduct([FromQuery]GetProductByIdQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    [HttpGet("Likes")]
    public async Task<IActionResult> GetLikes([FromQuery]GetLikesCountQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
    
    [HttpGet("LikeState")]
    public async Task<IActionResult> GetLikes([FromQuery]GetUserLikeStateQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("UpdateLike")]
    [Authorize]
    public async Task<IActionResult> EditLike(AddOrDeleteLikeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("files")]
   
    public async Task<IActionResult> GetAllFestivalFiles([FromQuery] GetAllFestivalFileQuery query)
    {      
        var result = await Mediator.Send(query);
        return Ok(result);
    }
}
