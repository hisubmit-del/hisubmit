using HiSubmit.Application.Features.MonetaryUnits.Queries;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Catalog
{
    public class MonetaryUnitsController : BaseApiController<MonetaryUnitsController>
    {

        /// <summary>
        /// Get all monetary units
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery]GetAllMonetaryUnitQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}
