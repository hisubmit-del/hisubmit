using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Features.Locatuions.Countries.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HiSubmit.Server.Controllers.v1.Location
{
    public class LocationController : BaseApiController<LocationController>
    {
        [HttpGet("GetAllCountries")]
        public async Task<IActionResult> GetAllCountries([FromQuery]GetAllCountryQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost("AddAddress")]
        public async Task<IActionResult> AddAddress(AddEditAddressCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
