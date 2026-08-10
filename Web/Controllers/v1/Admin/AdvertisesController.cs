using HiSubmit.Application.Features.Advertises.Commands;
using HiSubmit.Application.Features.Advertises.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.v1.Admin;

public class AdvertisesController : BaseAdminController<AdvertisesController>
{
    [HttpGet("GetAll")]
    [Authorize(Policy = Permissions.Advertise.RequestView)]
    public async Task<IActionResult> GetAll([FromQuery] GetAllAdvertiseQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpGet("Detail")]
    [Authorize(Policy = Permissions.Advertise.RequestView)]
    public async Task<IActionResult> GetDetail([FromQuery] GetDetailAdvertiseQuery query)
    {
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("AddBanner")]
    [Authorize(Policy = Permissions.Advertise.BannerUpdate)]
    public async Task<IActionResult> AddBanner(AddEditAdvertiseBannerCommand bannerCommand)
    {
        return Ok(await Mediator.Send(bannerCommand));
    }

    [HttpGet("AllBanner")]
    [Authorize(Policy = Permissions.Advertise.BannerView)]
    public async Task<IActionResult> AllBanner([FromQuery] GetAllAdvertiseBannerQuery query)
    {
        return Ok(await Mediator.Send(query));
    }

    [HttpDelete("DeleteBanner")]
    [Authorize(Policy = Permissions.Advertise.BannerUpdate)]
    public async Task<IActionResult> DeleteBanner([FromQuery] DeleteAdvertiseBannerCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}