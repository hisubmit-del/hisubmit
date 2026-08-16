using System.Threading.Tasks;
using HiSubmit.Application.Features.FooterItems.Commands;
using HiSubmit.Application.Features.FooterItems.Queries.GetAll;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Admin;

public class FooterItemController : BaseAdminController<FooterItemController>
{
   [HttpGet("getAll")]
   [Authorize(Permissions.Contents.View)]
   public async Task<IActionResult> GetAll([FromQuery] GetAllFooterItemQuery query)
   {
      return Ok(await Mediator.Send(query));
   }

   [HttpPost("Save")]
   [Authorize(Permissions.Contents.UpdateMenuItem)]
   public async Task<IActionResult> Save(AddEditFooterItemCommand command)
   {
      return Ok(await Mediator.Send(command));
   }

   [HttpDelete("delete")]
   [Authorize(Permissions.Contents.UpdateMenuItem)]
   public async Task<IActionResult> Delete([FromQuery]DeleteFooterItemCommand command)
   {
      return Ok(await Mediator.Send(command));
   }
}
