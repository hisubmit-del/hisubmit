using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Festival.SubUsers;

public partial class FestivalSubUserRoles
{
    [Inject]
    private  IFestivalSubUserManager FestivalSubUserManager { get; set; }
    
    [Parameter]
    public  int FestivalIdParam { get; set; }
    
    [Parameter] public string Id { get; set; }
    [Parameter] public string Title { get; set; }
    [Parameter] public string Description { get; set; }
    public List<UserRoleModel> UserRolesList { get; set; } = new();

    private UserRoleModel _userRole = new();
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;

    private ClaimsPrincipal _currentUser;
    private bool _canEditUsers;
    private bool _canSearchRoles;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await AuthenticationManager.CurrentUser();
        // _canEditUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
        // _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search))
        //     .Succeeded;
        _canSearchRoles = true;
        _canEditUsers = true;

        var userId = Id;
        var result = await _userManager.GetAsync(userId);
        if (result.Succeeded)
        {
            var user = result.Data;
            if (user != null)
            {
                Title = $"{user.FirstName} {user.LastName}";
                Description = string.Format(Localize["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                var response = await FestivalSubUserManager.GetUserRolesAsync(user.Id,await  GetFestivalId());
                if (response.Succeeded)
                {
                    UserRolesList = response.Data.UserRoles;
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        _loaded = true;
    }


    private async  Task<int> GetFestivalId()
    {
        if (FestivalIdParam != 0)
        {
            return FestivalIdParam;
        }

        var localFestivalId =await _localStorage.GetItemAsync<int?>(StorageConstants.Local.FestivalId);
        return localFestivalId ?? 0;
    }
    private async Task SaveAsync()
    {
        var request = new UpdateUserRolesRequest()
        {
            UserId = Id,
            UserRoles = UserRolesList
        };
        var result = await FestivalSubUserManager.UpdateRolesAsync(request);
        if (result.Succeeded)
        {
            _snackBar.Add(result.Messages[0], Severity.Success);
            _navigationManager.NavigateTo("/festival/subUser/users");
        }
        else
        {
            foreach (var error in result.Messages)
            {
                _snackBar.Add(error, Severity.Error);
            }
        }
    }

    private bool Search(UserRoleModel userRole)
    {
        if (string.IsNullOrWhiteSpace(_searchString)) return true;
        if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        if (userRole.RoleDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
        {
            return true;
        }

        return false;
    }
}

