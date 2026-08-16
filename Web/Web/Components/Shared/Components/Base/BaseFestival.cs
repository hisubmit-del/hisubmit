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

        var festivalPermissions =
            currentUser.Claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission).ToList();

        var neededPolicy = $"{SelectedFestivalId}-{permission}";

        if (SelectedFestivalId == 0 || festivalPermissions.All(p => p.Value != neededPolicy))
            NavigationManager.NavigateTo("/forbidden");

        await Task.CompletedTask;
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
