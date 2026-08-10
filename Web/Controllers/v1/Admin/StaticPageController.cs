using System.Threading.Tasks;
using HiSubmit.Application.Features.StaticPages.Commands;
using HiSubmit.Application.Features.StaticPages.Queries;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DeleteStaticPageCommand = HiSubmit.Application.Features.StaticPages.Commands.DeleteStaticPageCommand;

namespace Web.Controllers.v1.Admin;

public class StaticPageController : BaseAdminController<StaticPageController>
{
    [HttpGet("GetAll")]
    [Authorize(Permissions.Contents.View)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllStaticPageQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("Detail")]
    [Authorize(Permissions.Contents.View)]
    public async Task<IActionResult> GetDetail([FromQuery]GetDetailStaticPageQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpDelete("delete")]
    [Authorize(Permissions.Contents.UpdateStaticPage)]
    public async Task<IActionResult> Delete([FromQuery] DeleteStaticPageCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("save")]
    [Authorize(Permissions.Contents.UpdateStaticPage)]
    public async Task<IActionResult> Save(AddEditStaticPageCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}