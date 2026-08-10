using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Web.Components.Account;


namespace Web.Components.Pages.Authentication
{
    public partial class Login
    {
        [Inject] private SignInManager<BlazorHeroUser> SignInManager { get; set; }
        [Inject] private UserManager<BlazorHeroUser> UserManager { get; set; }

        [Inject] private IdentityRedirectManager RedirectManager { get; set; }

        private string? errorMessage;

        [CascadingParameter]
        private HttpContext HttpContext { get; set; } = null!;

        //[SupplyParameterFromForm]
        public TokenRequest Input { get; set; } = new();

        [SupplyParameterFromQuery]
        private string? ReturnUrl { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var hh = HttpContext.User;
            var gb = 3;
            if (HttpMethods.IsGet(HttpContext.Request.Method))
            {
                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            }
        }

        public async Task LoginUser()
        {
            var hh = HttpContext.User;

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, set lockoutOnFailure: true
            var user = await UserManager.FindByEmailAsync(Input.Email);
            if (user != null)
            {
                var c = await UserManager.CheckPasswordAsync(user, Input.Password);
                if (c)
                {
                 var s=   await SignInManager.PasswordSignInAsync(user.UserName,Input.Password, true,true);
                    RedirectManager.RedirectTo(ReturnUrl);
                }
            }
            errorMessage = "Error: Invalid login attempt.";

            //if (result.Succeeded)
            //{
            //  //  Logger.LogInformation("User logged in.");
            //    RedirectManager.RedirectTo(ReturnUrl);
            //}
            //else if (result.RequiresTwoFactor)
            //{
            //    RedirectManager.RedirectTo(
            //        "Account/LoginWith2fa",
            //        new() { ["returnUrl"] = ReturnUrl, ["rememberMe"] = true });
            //}
            //else if (result.IsLockedOut)
            //{
            //   // Logger.LogWarning("User account locked out.");
            //    RedirectManager.RedirectTo("Account/Lockout");
            //}
            //else
            //{
                //errorMessage = "Error: Invalid login attempt.";
            //}
        }






        //[Inject]
        //private  SelectedAccountService SelectedAccountService { get; set; }


        //[Inject] private SignInManager<BlazorHeroUser> SignInManager { get; set; }
        //[Inject] private UserManager<BlazorHeroUser> UserManager { get; set; }

        //[Inject] private IdentityRedirectManager RedirectManager { get; set; }

        //private FluentValidationValidator _fluentValidationValidator;
        //private bool _validated = true;

        //[SupplyParameterFromForm]
        //private TokenRequest _tokenModel = new();
        //private bool _processing;

        //protected override async Task OnInitializedAsync()
        //{
        //    var state = await _stateProvider.GetAuthenticationStateAsync();
        //    var isAuthenticated = state.User.Identity!.IsAuthenticated;
        //    if (isAuthenticated)
        //    {
        //        _navigationManager.NavigateTo("/",false);
        //    }

        //    await base.OnInitializedAsync();
        //}

        //private async Task SubmitAsync()
        //{
        //    _processing = true;
        //    _validated= _fluentValidationValidator
        //        .Validate(options => { options.IncludeAllRuleSets(); });

        //    var user = await UserManager.FindByEmailAsync(_tokenModel.Email);
        //    if (user != null && await UserManager.CheckPasswordAsync(user, _tokenModel.Password))
        //    {
        //        await SignInManager.PasswordSignInAsync(user,_tokenModel.Password, isPersistent: false,lockoutOnFailure:true);

        //        _snackBar.Add(string.Format(Localize["Welcome {0}"], _tokenModel.Email), 
        //            Severity.Success);
        //        _MainLayoutService.UserLoginAccount();
        //        _navigationManager.NavigateTo("/", false);
        //    }
        //    else
        //    {
        //        //foreach (var message in result.Messages)
        //        //{
        //        //    _snackBar.Add(message, Severity.Error);
        //        //}
        //        //if (result.Data is { GoToVerification: true })
        //        //{
        //        //  await  _localStorage.SetItemAsync(StorageConstants.Local.EmailRegistered, _tokenModel.Email);
        //        //    _navigationManager.NavigateTo("/confirm-email");
        //        //}
        //    }
        //    _processing = false;
        //}


        //private bool _passwordVisibility;
        //private InputType _passwordInput = InputType.Password;
        //private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        //private void TogglePasswordVisibility()
        //{
        //    if (_passwordVisibility)
        //    {
        //        _passwordVisibility = false;
        //        _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        //        _passwordInput = InputType.Password;
        //    }
        //    else
        //    {
        //        _passwordVisibility = true;
        //        _passwordInputIcon = Icons.Material.Filled.Visibility;
        //        _passwordInput = InputType.Text;
        //    }
        //}
    }
}