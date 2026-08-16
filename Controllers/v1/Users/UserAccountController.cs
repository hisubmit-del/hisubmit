using System.Threading.Tasks;
using HiSubmit.Application.Features.SpecialAccounts.Queries;
using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.v1.Users;

public class UserAccountController(ICurrentUserService currentUserService) 
    : BaseApiController<UserAccountController>
{
    /// <summary>
   /// get user account status
   /// </summary>
   /// <param name="query"></param>
   /// <returns></returns>
   [HttpGet("AccountStatus")]
   public async Task<IActionResult> GetAll([FromQuery] GetUserAccountTypeQuery query)
   {
      query ??= new GetUserAccountTypeQuery();
      query.UserId 
         = currentUserService.UserId;
      return Ok(await Mediator.Send(query));
   }
}
