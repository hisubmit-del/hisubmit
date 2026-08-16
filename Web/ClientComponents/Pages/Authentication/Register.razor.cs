using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Features.Users.Commands.Register;

namespace ClientComponents.Pages.Authentication;

public partial class Register
{
    private bool _processing;
    private bool _validated = true;
    private bool _passwordVisibility;
    private InputType _passwordInput = InputType.Password;
    private RegisterUserCommand _registerUserModel = new();
    private FluentValidationValidator _fluentValidationValidator;
    private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
    
    
    private async Task SubmitAsync()
    {
        _processing = true;
        _validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        //default active user and email confirmed
        _registerUserModel.AutoConfirmEmail = true;
        _registerUserModel.ActivateUser = true;

        var response = await _userManager.RegisterUserAsync(_registerUserModel);
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            await _localStorage.SetItemAsync(StorageConstants.Local.EmailRegistered, _registerUserModel.Email);

            _navigationManager.NavigateTo("/confirm-email");
            _registerUserModel = new RegisterUserCommand();
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