using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using Web.Components.Account;

namespace Web.Components.Pages.Authentication
{
    public partial class Login
    {
        [Inject] private SignInManager<BlazorHeroUser> SignInManager { get; set; }
        [Inject] private UserManager<BlazorHeroUser> UserManager { get; set; }
        [Inject] private IdentityRedirectManager RedirectManager { get; set; }

        private string? errorMessage;
        private bool _rememberMe;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = null!;

        [SupplyParameterFromForm(FormName = "LoginForm")]
        public TokenRequest Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            if (HttpContext is not null && HttpMethods.IsGet(HttpContext.Request.Method))
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }

        public async Task LoginUser()
        {
            errorMessage = null;

            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user is null)
            {
                errorMessage = "Error: Invalid login attempt.";
                return;
            }

            var result = await SignInManager.PasswordSignInAsync(
                user.UserName ?? Input.Email,
                Input.Password,
                _rememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                RedirectManager.RedirectTo(ReturnUrl);
                return;
            }

            if (result.IsLockedOut)
            {
                errorMessage = "Error: Your account is locked. Please try again later.";
            }
            else if (result.RequiresTwoFactor)
            {
                RedirectManager.RedirectTo(
                    "Account/LoginWith2fa",
                    new()
                    {
                        ["returnUrl"] = ReturnUrl,
                        ["rememberMe"] = _rememberMe
                    });
                return;
            }
            else
            {
                errorMessage = "Error: Invalid login attempt.";
            }
        }
    }
}
