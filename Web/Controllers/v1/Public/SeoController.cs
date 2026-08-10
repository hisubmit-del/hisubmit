using System.Threading.Tasks;
using HiSubmit.Application.Features.Seo.GetPAgeSeoTags;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Public;

public class SeoController : BasePublicController<SeoController>
{
    // GET
    [HttpGet("PageSeoTag")]
    public async Task<IActionResult> Index([FromQuery] GetPageSeoTagsQuery query)
    {
        var res =await Mediator.Send(query);
        return Ok(res);
    }
}