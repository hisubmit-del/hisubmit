using System.Threading.Tasks;
using HiSubmit.Application.Features.Seo;
using Microsoft.AspNetCore.Mvc;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Application.Features.Seo.GetPAgeSeoTags;
using Hisubmit.Client.SharedModels.Features.Seo;

namespace Web.Controllers.v1.Festival;

public class SeoController : BaseFestivalController<SeoController>
{
    // GET
    [HttpGet("SeoTags")]
    
    public async Task<IActionResult> GetSeoTags(int festivalId)
    {
        var f = new GetPageSeoTagsQuery()
        {
            PageId = festivalId.ToString(),
            PageType = PageType.FestivalPage
        };
        return Ok(await Mediator.Send(f));
    }
    
    [HttpPost("SeoTagsSetting")]
    public async Task<IActionResult> UpdateSeoTags(AddEditSeoTagCommand request,int festivalId)
    {
        request.PageId = festivalId.ToString();
        return Ok(await Mediator.Send(request));
    }
}
