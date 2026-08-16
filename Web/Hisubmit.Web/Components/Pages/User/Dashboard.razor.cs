using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.SpecialAccounts.Queries;
using HiSubmit.Client.Infrastructure.Managers.UsersAccounts;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Notifications;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Hisubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Features.Notifications.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Requests.Identity;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.User;

public partial class Dashboard
{
    [Inject] private IUserAccountManager UserAccountManager { get; set; }

    [Inject] private IProjectManager ProjectManager { get; set; }

    [Inject] private ISubmitManager SubmitManager { get; set; }
    [Inject] private INotificationManager UserNotificationManager { get; set; }

    private GetUserAccountTypeResponse _accountType = new();

    private List<GetAllProjectResponse> _projects = new();

    private List<GetAllSubmitsResponse> _submit = new List<GetAllSubmitsResponse>();
    private List<GetAllNotificationResponse> _notifications = new List<GetAllNotificationResponse>();
    private UpdateProfileRequest _profileModel=new();

    private string UserId;
    private string ImageDataUrl;
    private bool _userLoaded;
    private bool _projectLoaded;
    private bool _submitLoaded;
    private bool _notificationLoaded;
    private bool isAdmin=true;
    protected override async Task OnInitializedAsync()
    {
        isAdmin = (await AuthenticationManager.CurrentUser()).IsInRole(RoleConstants.AdministratorRole);

        await base.OnInitializedAsync();
        await LoadDataAsync();
        await LoadAccountStatus();
        _userLoaded = true;
        await LoadProjects();
        _projectLoaded=true;

        await LoadSubmits();
        _submitLoaded = true;
        await LoadNotification();
        _notificationLoaded=true;
    }


    private async Task LoadAccountStatus()
    {
        var response =
            await UserAccountManager.GetAccountType(new GetUserAccountTypeQuery
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
    }

    private async Task LoadProjects()
    {
        var response = await ProjectManager
            .GetAllAsync(new GetAllProjectRequest()
            {
                UserId = UserId,
                GetCurrentUserProjects = true,
                PageNumber = 1,
                PageSize = 8
            });
        if (response.Succeeded)
        {
            _projects = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadSubmits()
    {
        var response = await SubmitManager
            .GetAll(new GetAllSubmitsRequest()
            {
                UserId = UserId,
                PageNumber = 1,
                PageSize = 3
            });
        if (response.Succeeded)
        {
            _submit = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadNotification()
    {
        var response = await UserNotificationManager
            .GetUserNotifications(new GetAllNotificationQuery()
            {
                UserId = UserId,
                PageNumber = 1,
                PageSize = 10
            });
        if (response.Succeeded)
        {
            _notifications = response.Data;
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

