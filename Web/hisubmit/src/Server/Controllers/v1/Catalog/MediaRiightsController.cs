using HiSubmit.Application.Features.MediaRights.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Catalog;

public class MediaRightController : BaseApiController<MediaRightController>
{
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMediaRightQuery query)
    {
        return Ok(await Mediator.Send(new GetAllMediaRightQuery()));
    }
}
