using System;
using MudBlazor;
using System.Linq;
using Blazored.LocalStorage;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.RemoveUserFromFestival;
using Web.Components.Pages.Identity;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Responses.Identity;
using Web.Components.Pages.Festival.JudgingProjects;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using Hisubmit.Client.SharedModels.Features.SubUsers.Queries.GetFestivalUsers;

namespace Web.Components.Pages.Festival.SubUsers;

public partial class FestivalSubUser
{
    #region Inject

    [Inject] public ILocalStorageService LocalStorageService { get; set; }

    [Inject] public IFestivalSubUserManager FestivalSubUserManager { get; set; }

    #endregion

    #region Private Field

    private List<UserResponse> _userList = new();
    private UserResponse _user = new();
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private ClaimsPrincipal _currentUser;
    private bool _loaded;
    
    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.CheckPermission(Permissions.SubUser.View);
        _currentUser = await AuthenticationManager.CurrentUser();

        await GetUsersAsync();
        _loaded = true;
    }

    private async Task GetUsersAsync()
    {
        var response = await FestivalSubUserManager
            .GetFestivalUserAsync(new GetFestivalSubUserQuery() { FestivalId = SelectedFestivalId });
        if (response.Succeeded)
        {
            _userList = response.Data.ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
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

    // private async Task ExportToExcel()
    // {
    //     var base64 = await _userManager.ExportToExcelAsync(_searchString);
    //     await _jsRuntime.InvokeVoidAsync("Download", new
    //     {
    //         ByteArray = base64,
    //         FileName = $"{nameof(FestivalSubUser).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
    //         MimeType = ApplicationConstants.MimeTypes.OpenXml
    //     });
    //     _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
    //         ? _localizer["Users exported"]
    //         : _localizer["Filtered Users exported"], Severity.Success);
    // }

    private async Task InvokeModal()
    {
        await LoadSelectedFestivalId();
        var parameters = new DialogParameters
        {
            { nameof(RegisterUserModal.IsFestivalUser), true },
            { nameof(RegisterUserModal.FestivalId), SelectedFestivalId }
        };
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
        else _navigationManager.NavigateTo($"/festival/{SelectedFestivalId}/subUser/user-roles/{userId}");
    }

    private async Task AddToProjects(string userId)
    {
        var festivalId = await LocalStorageService.GetItemAsync<int>(StorageConstants.Local.FestivalId);

        var parameters = new DialogParameters
        {
            { nameof(AddProjectsToRefree.FestivalId), festivalId },
            { nameof(AddProjectsToRefree.RefereeId), userId }
        };
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddProjectsToRefree>(Localize["Add Judge To Projects"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await GetUsersAsync();
        }
    }

    private async Task AddToFestival()
    {
        var parameters = new DialogParameters
        {
            { nameof(AddExistingUserToFestivalModal.FestivalId), SelectedFestivalId }
        };
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog =
            _dialogService.Show<AddExistingUserToFestivalModal>(Localize["Add User To festival"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await GetUsersAsync();
        }
    }

    private async Task RemoveFromFestival(string contextId)
    {
        var result = await FestivalSubUserManager
            .RemovedUserFromFestival(new RemoveUserFromFestivalCommand
        {
            UserId = contextId,
            FestivalId = SelectedFestivalId
        },SelectedFestivalId);
        
        _snackBar.Add(result.Messages[0], result.Succeeded ? Severity.Success : Severity.Error);
    }
}