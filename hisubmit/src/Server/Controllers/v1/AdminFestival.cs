using HiSubmit.Application.Features.AdminFestival.Commands.UpdateFestivalState;
using HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Application.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;
using HiSubmit.Application.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace HiSubmit.Server.Controllers.v1;

public class AdminFestivalController : BaseAdminController<AdminFestivalController>
{

    /// <summary>
    /// get all festival registerd in site 
    /// </summary>
    /// <param name="query"></param>
    /// <returns>
    /// all festival pass filter count of pagination setting 
    /// </returns>
    [HttpPost("GetAll")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(GetAllFestivalQuery query)
    {
        return Ok(await Mediator.Send(query));
    }


    /// <summary>
    /// update festival state (active or deactive festival)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPost("UpdateState")]
    [Authorize(Policy = Permissions.AdminFestival.Activate)]
    public async Task<IActionResult> UpdateState(UpdateFestivalStateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }



        
    /// <summary>
    /// update fee status (special or usual)
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [HttpPut("UpdateFeeStatus")]
    [Authorize(Policy = Permissions.AdminFestival.ChangeFeeType)]
    public async Task<IActionResult> UpdateFeeStatus(UpdateFestivalFeeStatusCommand command)
    {
        return Ok(await Mediator.Send(command));
    }



    /// <summary>
    /// ProductFestivalId Detail
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet("Detail")]
    [Authorize(Policy = Permissions.AdminFestival.View)]
    public async Task<IActionResult> GetFestivalDetail([FromQuery]GetFestivalDetailByIdQuery query)
    {
        return Ok(await  Mediator.Send(query));
    }
}