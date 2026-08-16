using System.Threading.Tasks;
using HiSubmit.Application.Features.MasterFestivals.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Admin;

public class FestivalMasterController:BaseAdminController<FestivalMasterController>
{
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllMasterFestivalQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}