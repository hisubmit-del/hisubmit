using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Application.Features.Payments.DiscountsCodes.Commands;
using HiSubmit.Server.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.Payments.DiscountsCodes.Queries;

namespace HiSubmit.Server.Controllers.v1.Festival;

public class DiscountCodeController : BaseFestivalController<DiscountCodeController>
{
    [HttpPost("AddEdit")]
    [FestivalAuthentication(Policy = Permissions.DiscountCode.Edit)]

    public async Task<IActionResult> AddEdit(AddEditDiscountCodeCommand command,int festivalId)
    {
        command.FestivalId=festivalId;
        return Ok(await Mediator.Send(command));
    }


    [HttpPost("GetAll")]
    [FestivalAuthentication(Policy = Permissions.DiscountCode.View)]
    public async Task<IActionResult> GetAll(GetAllDiscountCodeQuery query,int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpPost("ChangeStatus")]
    [FestivalAuthentication(Policy = Permissions.DiscountCode.Edit)]
    public async Task<IActionResult> ChangeStatus(ChangeDiscountCodeStatusQuery query, int festivalId)
    {
        query.FestivalId = festivalId;
        return Ok(await Mediator.Send(query));
    }

    [HttpDelete("Delete")]
    [FestivalAuthentication(Policy = Permissions.DiscountCode.Edit)]
    public async Task<IActionResult> Delete([FromQuery]DeleteDiscountCodeCommand command, int festivalId)
    {
        command.FestivalId = festivalId;
        return Ok(await Mediator.Send(command));
    }

}