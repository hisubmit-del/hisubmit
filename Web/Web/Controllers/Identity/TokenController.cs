using HiSubmit.Application.Features.Users.Commands.ResendVerificationEmail;
using HiSubmit.Application.Features.Users.Commands.VerifyAccount;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Identity.Users;
using HiSubmit.Infrastructure.Models.Identity;

namespace Web.Controllers.Identity;

[Route("api/identity/token")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _identityService;
    private UserManager<BlazorHeroUser> _userManager;
    private SignInManager<BlazorHeroUser> _signInManager;
    private readonly IMediator _mediator;

    public TokenController(ITokenService identityService,IMediator mediator
        , ICurrentUserService currentUserService,UserManager<BlazorHeroUser> userManager,SignInManager<BlazorHeroUser> signInManager)
    {
        _mediator=mediator;
        _userManager=userManager;
        _signInManager=signInManager;
        _identityService = identityService; 
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(TokenRequest model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(Result.Success());
        }

        return Ok(Result.Fail("Invalid credentials"));
    }





    /// <summary>
    /// Get Token (Email, Password)
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpPost]
    public async Task<ActionResult> Get(TokenRequest model)
    {
        var response = await _identityService.LoginAsync(model);
        return Ok(response);
    }

    /// <summary>
    /// Refresh Token
    /// </summary>
    /// <param name="model"></param>
    /// <returns>Enable 200 OK</returns>
    [HttpPost("refresh")]
    public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
    {
        var response = await _identityService.GetRefreshTokenAsync(model);
        return Ok(response);
    }


    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyAccount(VerifyAccountCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("resend-email")]
    public async Task<IActionResult> ResendCode(ResendVerificationCodeCommand command)
    {
        return Ok(await _mediator.Send(command));
    }

}