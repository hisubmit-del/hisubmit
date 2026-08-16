using System.Threading.Tasks;
using HiSubmit.Application.Features.Emails;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Public;

public class EmailController:BaseApiController<EmailController>
{
    [HttpGet("SendEmail")]
    public async Task<IActionResult> Send()
    {
        return Ok(await Mediator.Send(new EmailSender()));
    }
}