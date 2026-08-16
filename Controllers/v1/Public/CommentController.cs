using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Comments.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Public;

public class CommentController:BasePublicController<CommentController>
{
    [HttpPost("Add")]
    public async Task<IActionResult> AddComment(AddCommentCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
    
    
}