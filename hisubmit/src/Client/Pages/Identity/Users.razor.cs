using System;
using MudBlazor;
using System.Linq;
using Microsoft.JSInterop;
using Hisubmit.Client.SharedModels.Enums;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Responses.Identity;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Services;

namespace HiSubmit.Client.Pages.Identity
{
    public partial class Users
    {
        private List<UserResponse> _userList = new();
        private UserResponse _user = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canCreateUsers;
        private bool _canSearchUsers;
        private bool _canExportUsers;
        private bool _canViewRoles;
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            await  base.OnInitializedAsync();
            _currentUser = await AuthenticationManager.CurrentUser();
            _canCreateUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Create))
                .Succeeded;
            _canSearchUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Search))
                .Succeeded;
            _canExportUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Export))
                .Succeeded;
            _canViewRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.View))
                .Succeeded;

            await GetUsersAsync();
            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await SeenNotification();
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task SeenNotification()
        {
            await NotificationManager.SeenNotifications(new SeenNotificationCommand
            {
                AccountType = SiteAccountType.Admin,
                NotificationTypes = NotificationType.AdminNewRegister
            });
            NotificationService.ChangeNotificationBar();
        }

        private async Task GetUsersAsync()
        {
            var response = await _userManager.GetAllAsync();
            if (response.Succeeded)
            {
                _userList = response.Data.ToList();
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }

        private bool Search(UserResponse user)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (user.FirstName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (user.LastName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (user.Email?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (user.PhoneNumber?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            if (user.UserName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }

            return false;
        }

        private async Task ExportToExcel()
        {
            var base64 = await _userManager.ExportToExcelAsync(_searchString);
            await _jsRuntime.InvokeVoidAsync("Download", new
            {
                ByteArray = base64,
                FileName = $"{nameof(Users).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                MimeType = ApplicationConstants.MimeTypes.OpenXml
            });
            _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                ? Localize["Users exported"]
                : Localize["Filtered Users exported"], Severity.Success);
        }

        private async Task InvokeModal()
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions
                { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<RegisterUserModal>(Localize["Register New User"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                await GetUsersAsync();
            }
        }

        private void ViewProfile(string userId)
        {
            _navigationManager.NavigateTo($"/user-profile/{userId}");
        }

        private void ManageRoles(string userId, string email)
        {
            if (email == "mukesh@blazorhero.com") _snackBar.Add(Localize["Not Allowed."], Severity.Error);
            else _navigationManager.NavigateTo($"/identity/user-roles/{userId}");
        }
    }
}