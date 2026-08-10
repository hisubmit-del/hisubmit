using System;
using Hisubmit.Client.SharedModels.Enums;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.Infrastructure.Constants;
using Hisubmit.Client.SharedModels.Features.Notifications.Queries;
using HiSubmit.Client.Infrastructure.Managers.Notifications;

namespace ClientComponents.Shared.Components;

public partial class NotificationDropDown
{
    #region Private Field

    private ClaimsPrincipal _currentUser;
    private List<GetAllNotificationResponse> _items = new();

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadNotifications();
        NotificationService.NotificationSeen += async (s, h) => await OnChangeNotification(s, h);
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task OnChangeNotification(object? e, EventArgs a)
    {
        await LoadNotifications();
        StateHasChanged();
    }

    private async Task LoadNotifications()
    {
        _items.Clear();
        _currentUser = await AuthenticationManager.CurrentUser();
        if (_currentUser.IsInRole(RoleConstants.AdministratorRole))
            await LoadAdminNotification();
        else
        {
            if (await _localStorage.GetItemAsync<int?>(StorageConstants.Local.SelectedFestivalId) == null)
            {
                await LoadUserNotification();
            }
            else
            {
                await LoadFestivalNotification();
            }
        }
    }

    private async Task LoadUserNotification()
    {
        var response = await NotificationManager.GetUserNotifications(new GetAllNotificationQuery
        {
            Seen = false,
            GetAllData = true,
            SiteAccountType = SiteAccountType.User,
            UserId = _currentUser.FindFirstValue(ClaimTypes.NameIdentifier),
        });
        if (response.Succeeded)
            _items.AddRange(response.Data);
    }

    private async Task LoadFestivalNotification()
    {
        var festivalId = await _localStorage.GetItemAsync<int>(StorageConstants.Local.SelectedFestivalId);
        var response = await NotificationManager.GetFestivalNotifications(new GetAllNotificationQuery
        {
            Seen = false,
            GetAllData = true,
            FestivalId = festivalId,
            SiteAccountType = SiteAccountType.Festival,
        });
        if (response.Succeeded)
            _items.AddRange(response.Data);
    }

    private async Task LoadAdminNotification()
    {
        var response = await NotificationManager.GetAdminNotifications(new GetAllNotificationQuery
        {
            Seen = false,
            GetAllData = true,
            SiteAccountType = SiteAccountType.Admin,
        });
        if (response.Succeeded)
            _items.AddRange(response.Data);
    }
}