using HiSubmit.Application.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Domain.Entities.Projects;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Catalog
{
    public class SubProjectTypeController : BaseApiController<SubProjectTypeController>
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllSubProjectTypeQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
