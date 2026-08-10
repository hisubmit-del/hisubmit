using HiSubmit.Application.Features.FestivalQualifyers.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Catalog;

public class FestivalQualifiersController : BaseApiController<FestivalQualifiersController>
{
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromQuery]GetAllFestivalQualifiersQuery query)
    {
        return Ok(await Mediator.Send(query));
    }
}
