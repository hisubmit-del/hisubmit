using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace Web.Controllers
{
    [Route("api/identity/roleClaim")]
    [ApiController]
    public class RoleClaimController(IRoleClaimService roleClaimService) : ControllerBase
    {
        /// <summary>
        /// Get All Role Claims(e.g. Product Create Permission)
        /// </summary>
        /// <returns>Enable 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roleClaims = await roleClaimService.GetAllAsync();
            return Ok(roleClaims);
        }

        /// <summary>
        /// Get All Role Claims By Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Enable 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetAllByRoleId([FromRoute] string roleId)
        {
            var response = await roleClaimService.GetAllByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Add a Role Claim
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Enable 200 OK </returns>
        [Authorize(Policy = Permissions.RoleClaims.Create)]
        [HttpPost]
        public async Task<IActionResult> Post(RoleClaimRequest request)
        {
            var response = await roleClaimService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a Role Claim
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Enable 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.Delete)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await roleClaimService.DeleteAsync(id);
            return Ok(response);
        }
    }
}