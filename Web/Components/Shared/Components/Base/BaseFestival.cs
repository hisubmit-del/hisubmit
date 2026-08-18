using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Shared.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Web.Components.Shared.Components.Base;

public class BaseFestival : ComponentBase
{
    #region Inject

    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private IAuthenticationManager AuthenticationManager { get; set; }
    [Inject] private MainLayoutService MainLayoutService { get; set; }

    #endregion

    //protected override Task OnInitializedAsync()
    //{
    //    MainLayoutService.ChangeDrawerStatus(true);
    //    return base.OnInitializedAsync();
    //}

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        MainLayoutService.ChangeDrawerStatus(true);
        await base.OnAfterRenderAsync(firstRender);
    }

    protected int SelectedFestivalId { get; set; }
    private int FestivalId { get; set; }

    protected async Task CheckPermission(string permission)

    {
        await LoadSelectedFestivalId();

        if (FestivalId == SelectedFestivalId && FestivalId != 0)
        {
            return;
        }

        var currentUser = await AuthenticationManager.CurrentUser();

        if (currentUser.IsInRole(RoleConstants.AdministratorRole)) return;

        if (!HasFestivalPermission(currentUser, SelectedFestivalId, permission))
            NavigationManager.NavigateTo("/forbidden");

        await Task.CompletedTask;
    }

    private static bool HasFestivalPermission(ClaimsPrincipal user, int festivalId, string permission)
    {
        if (festivalId <= 0 || string.IsNullOrWhiteSpace(permission))
            return false;

        foreach (var claim in user.Claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission))
        {
            try
            {
                var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(claim.Value);
                if (permissions != null &&
                    permissions.TryGetValue(festivalId, out var festivalPolicies) &&
                    festivalPolicies?.Any(policy =>
                        string.Equals(policy, permission, StringComparison.OrdinalIgnoreCase)) == true)
                {
                    return true;
                }
            }
            catch (JsonException)
            {
                // Invalid claims must never grant access.
            }
        }

        return false;
    }

    protected async Task LoadSelectedFestivalId()
    {
        var user = await AuthenticationManager.CurrentUser();

        if (user.Claims.FirstOrDefault(p => p.Type == ApplicationClaimTypes.FestivalId) != null)
        {
            FestivalId=
                int.Parse(user.Claims.First(p => p.Type == ApplicationClaimTypes.FestivalId).Value); ;
        }

        if (await AuthenticationManager.GetAdminLoginToFestivalId()!=null)
        {
            SelectedFestivalId =
               await AuthenticationManager.GetAdminLoginToFestivalId() ?? 0;
        }
        else
        {
            var sId = await AuthenticationManager.GetSelectedFestivalId();

            if (sId!=null)
            {
                SelectedFestivalId = sId.Value;
            }
            else
            {
                 SetFestivalIdToSelectedFestivalId();
            }
        }
    }

    private void SetFestivalIdToSelectedFestivalId()
    {
        SelectedFestivalId = FestivalId;
    }
}
