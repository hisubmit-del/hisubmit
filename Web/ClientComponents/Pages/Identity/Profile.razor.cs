using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Requests.Identity;
using ClientComponents.Extensions;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SpecialAccounts.Queries;
using HiSubmit.Client.Infrastructure.Managers.UsersAccounts;
using Hisubmit.Client.SharedModels.Enums;

namespace ClientComponents.Pages.Identity
{
    public partial class Profile
    {
        [Inject] private IUserAccountManager UserAccountManager { get; set; }
        private FluentValidationValidator _fluentValidationValidator;

        private bool Validated =>
            _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        private char _firstLetterOfName;
        private readonly UpdateProfileRequest _profileModel = new();

        private GetUserAccountTypeResponse _accountType = new();
        public string UserId { get; set; }

        private async Task UpdateProfileAsync()
        {
            var response = await _accountManager.UpdateProfileAsync(_profileModel);
            if (response.Succeeded)
            {
                await AuthenticationManager.Logout();
                _snackBar.Add(_localizer["Your Profile has been updated. Please Login to Continue."], Severity.Success);
                _navigationManager.NavigateTo("/");
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await LoadAccountStatus();
            }
        }

        private async Task LoadAccountStatus()
        {
            var response = await UserAccountManager.GetAccountType(new GetUserAccountTypeQuery
            {
                UserId = ""
            });
            if (response.Succeeded)
                _accountType = response.Data;
            else
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            StateHasChanged();
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            _profileModel.Email = user.GetEmail();
            _profileModel.FirstName = user.GetFirstName();
            _profileModel.LastName = user.GetLastName();
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            UserId = user.GetUserId();
            var data = await _accountManager.GetProfilePictureAsync(UserId);
            if (data.Succeeded)
            {
                ImageDataUrl = data.Data;
            }

            if (_profileModel.FirstName.Length > 0)
            {
                _firstLetterOfName = _profileModel.FirstName[0];
            }
        }


        private IBrowserFile _file;

        [Parameter] public string ImageDataUrl { get; set; }

        private async Task UploadFiles(InputFileChangeEventArgs e)
        {
            _file = e.File;
            if (_file != null)
            {
                var extension = Path.GetExtension(_file.Name);
                var fileName = $"{UserId}-{Guid.NewGuid()}{extension}";
                var format = "image/png";
                var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
                var buffer = new byte[imageFile.Size];
                await imageFile.OpenReadStream().ReadAsync(buffer);
                var request = new UpdateProfilePictureRequest
                {
                    Data = buffer, FileName = fileName, Extension = extension,
                    UploadType =UploadType.ProfilePicture
                };
                var result = await _accountManager.UpdateProfilePictureAsync(request, UserId);
                if (result.Succeeded)
                {
                    await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data);
                    _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
                    _navigationManager.NavigateTo("/account", true);
                }
                else
                {
                    foreach (var error in result.Messages)
                    {
                        _snackBar.Add(error, Severity.Error);
                    }
                }
            }
        }

        private async Task DeleteAsync()
        {
            var parameters = new DialogParameters
            {
                {
                    nameof(Shared.Dialogs.DeleteConfirmation.ContentText),
                    $"{string.Format(_localizer["Do you want to delete the profile picture of {0}"], _profileModel.Email)}?"
                }
            };
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog =
                _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                var request = new UpdateProfilePictureRequest
                    { Data = null, FileName = string.Empty, UploadType = UploadType.ProfilePicture };
                var data = await _accountManager.UpdateProfilePictureAsync(request, UserId);
                if (data.Succeeded)
                {
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
                    ImageDataUrl = string.Empty;
                    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
                    _navigationManager.NavigateTo("/account", true);
                }
                else
                {
                    foreach (var error in data.Messages)
                    {
                        _snackBar.Add(error, Severity.Error);
                    }
                }
            }
        }
    }
}