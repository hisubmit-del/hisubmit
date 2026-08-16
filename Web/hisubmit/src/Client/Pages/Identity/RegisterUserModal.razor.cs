using System;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Users.Commands.Register;
using Hisubmit.Client.SharedModels.Requests.Identity;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Identity
{
    public partial class RegisterUserModal
    {
        [Inject]
        public IFestivalSubUserManager FestivalSubUserManager { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated = true;
        private readonly RegisterUserCommand _registerUserModel = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

        [Parameter]
        public bool IsFestivalUser { get; set; }
        [Parameter]
        public  int? FestivalId { get; set; }


        private bool _processing;
        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SubmitAsync()
        {
            _registerUserModel.UserName = _registerUserModel.Email;
            Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
            if(!Validated)
                return;
            
            _processing = true;
            _registerUserModel.IsFestivalUser = IsFestivalUser;
            _registerUserModel.FestivalId = FestivalId;
            IResult response;
            if (IsFestivalUser)
            {
                response = await FestivalSubUserManager.SaveUser(_registerUserModel);
            }
            else
            {             
                response = await _userManager.RegisterUserAsync(_registerUserModel);
            }

            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
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


