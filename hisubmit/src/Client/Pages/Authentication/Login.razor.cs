using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Authentication
{
    public partial class Login
    {
        [Inject]
        private  SelectedAccountService SelectedAccountService { get; set; }
        [Inject]
        private SignInManager<BlazorHeroUser> SignInManager { get; set; }

        [Inject]
        private UserManager<BlazorHeroUser> UserManager { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool _validated = true;
        private TokenRequest _tokenModel = new();
        private bool _processing;

        protected override async Task OnInitializedAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var isAuthenticated = state.User.Identity!.IsAuthenticated;
            if (isAuthenticated)
            {
                _navigationManager.NavigateTo("/",false);
            }

            await base.OnInitializedAsync();
        }

        private async Task SubmitAsync()
        {
            _processing = true;
            _validated= _fluentValidationValidator
                .Validate(options => { options.IncludeAllRuleSets(); });

            var user = await UserManager.FindByEmailAsync(_tokenModel.Email);
            if (user != null && await UserManager.CheckPasswordAsync(user, _tokenModel.Password))
            {
                await SignInManager.PasswordSignInAsync(user,_tokenModel.Password, isPersistent: false,lockoutOnFailure:true);
          
                _snackBar.Add(string.Format(Localize["Welcome {0}"], _tokenModel.Email), 
                    Severity.Success);
                _MainLayoutService.UserLoginAccount();
                _navigationManager.NavigateTo("/", false);
            }
            else
            {
                //foreach (var message in result.Messages)
                //{
                //    _snackBar.Add(message, Severity.Error);
                //}
                //if (result.Data is { GoToVerification: true })
                //{
                //  await  _localStorage.SetItemAsync(StorageConstants.Local.EmailRegistered, _tokenModel.Email);
                //    _navigationManager.NavigateTo("/confirm-email");
                //}
            }
            _processing = false;
        }


        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }
    }
}