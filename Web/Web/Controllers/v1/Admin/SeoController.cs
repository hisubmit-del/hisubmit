using System.Threading.Tasks;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Features.Seo.GetPAgeSeoTags;
using Hisubmit.Client.SharedModels.Features.Seo;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin;

public class SeoController : BaseAdminController<SeoController>
{
    // GET
    [HttpPost("SeoSetting")]
    public async Task<IActionResult> AddEditSeo(AddEditSeoTagCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
    
    [HttpGet("PageSeoTag")]
    public async Task<IActionResult> Index([FromQuery] GetPageSeoTagsQuery query)
    {
        var res =await Mediator.Send(query);
        return Ok(res);
    }
}