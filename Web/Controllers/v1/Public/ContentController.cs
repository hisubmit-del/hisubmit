using System.Threading.Tasks;
using HiSubmit.Application.Features.FestivalLikes;
using HiSubmit.Application.Features.FooterItems.Queries.GetAll;
using HiSubmit.Application.Features.News.Queries;
using HiSubmit.Application.Features.StaticPages.Queries;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Public;

public class ContentController : BasePublicController<ContentController>
{
    /// <summary>
    /// get All news
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("News")]
    public async Task<IActionResult> GetAllNews([FromQuery] GetAllNewQuery query)
    {
        query.IsEnable= true;
        return Ok(await Mediator.Send(query));
    }

    /// <summary>
    /// get new detail
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("New")]
    public async Task<IActionResult> GetDetailNew([FromQuery] GetDetailNewQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("FooterItems")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllFooterItemQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("staticPage")]
    public async Task<IActionResult> StaticPage([FromQuery] GetDetailStaticPageQuery query)
    {
        query.IsEnable = true;
        query.Id = 0;
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("GetFAQ")]
    public async Task<IActionResult> GetFAQ([FromQuery] GetAllStaticPageQuery query)
    {
        query.IsEnable = true;
        query.Type = ContentType.Faq;
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("UpdateLike")]
    [Authorize]
    public async Task<IActionResult> EditLike(AddOrDeleteLikeCommand command)
    {
        return Ok(await Mediator.Send(command));
    }


    [HttpGet("Likes")]
    public async Task<IActionResult> GetLikes([FromQuery] GetLikesCountQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("LikeState")]
    public async Task<IActionResult> GetLikes([FromQuery] GetUserLikeStateQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
