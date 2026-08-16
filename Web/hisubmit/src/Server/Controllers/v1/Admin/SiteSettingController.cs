using System.Threading.Tasks;
using HiSubmit.Application.Features.Payments.Commands.EditSiteCommission;
using HiSubmit.Application.Features.Payments.Queries;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiSubmit.Server.Controllers.v1.Admin;

public class SiteSettingController : BaseAdminController<SiteSettingController>
{

    /// <summary>
    /// get all site commission
    /// </summary>
    /// <returns></returns>
    [HttpGet("Commissions")]
    [Authorize(Policy = Permissions.Commission.View)]
    public async Task<IActionResult> GetSiteCommission()
    {
        return Ok(await Mediator.Send(new GetSiteCommissionQuery()));
    }


    /// <summary>
    /// update site commission
    /// </summary>
    /// <returns></returns>
    [HttpPost("UpdateCommissions")]
    [Authorize(Policy = Permissions.Commission.Update)]
    public async Task<IActionResult> UpdateCommission(EditSiteCommissionCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}