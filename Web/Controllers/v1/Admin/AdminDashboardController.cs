using HiSubmit.Application.Features.AdminDashboard.Queries;
using HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin;

public class AdminDashboardController : BaseAdminController<DashboardController>
{
    [HttpGet("GetAccountStatusCount")]
    public async Task<IActionResult> GetStatus()
    {
        return Ok(await Mediator.Send(new GetFestivalAndUserStatusCountQuery()));
    }


    [HttpGet("GetUnderInvestigationFestival")]
    public async Task<IActionResult> GetUndertInvestigationFestival()
    {
        return Ok(await Mediator.Send(new GetAllFestivalQuery()
        {
            FestivalStatus = FestivalStatus.UnderInvestigation,
            PageNumber = 1,
            PageSize = 4
        }));
    }


    [HttpGet("Purchase")]
    public async Task<IActionResult> GetPurchaseRequest([FromQuery]GetSitePurchaseQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
