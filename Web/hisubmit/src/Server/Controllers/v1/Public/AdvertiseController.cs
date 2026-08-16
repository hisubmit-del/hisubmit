using System.Threading.Tasks;
using HiSubmit.Application.Features.Advertises.Commands;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Public;

public class AdvertiseController:BasePublicController<AdvertiseController>
{
    [HttpPost("AddRequest")]
    public async Task<IActionResult> AddRequest(AddAdvertiseCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
    
    
}