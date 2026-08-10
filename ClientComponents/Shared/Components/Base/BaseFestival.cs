using System.Linq;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.Infrastructure.Services;
using System;

namespace ClientComponents.Shared.Components.Base;

public class BaseFestival : ComponentBase
{
    #region Inject

    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private IAuthenticationManager AuthenticationManager { get; set; }
    [Inject] private MainLayoutService MainLayoutService { get; set; }

    #endregion

    protected override Task OnInitializedAsync()
    {
        MainLayoutService.ChangeDrawerStatus(true);
        return base.OnInitializedAsync();
    }

    protected int SelectedFestivalId { get; set; }
    private int FestivalId { get; set; }

    protected async Task CheckPermission(string permission)

    {
        await LoadSelectedFestivalId();
        Console.WriteLine("Check Permission 1");
        if (FestivalId == SelectedFestivalId && FestivalId != 0)
        {
            return;
        }
        Console.WriteLine("Check Permission 2");
        var currentUser = await AuthenticationManager.CurrentUser();

        if (currentUser.IsInRole(RoleConstants.AdministratorRole)) return;
        Console.WriteLine("Check Permission 3");
        var festivalPermissions =
            currentUser.Claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission).ToList();
        Console.WriteLine("Check Permission 4");
        var neededPolicy = $"{SelectedFestivalId}-{permission}";
        Console.WriteLine("Check Permission 5");
        if (SelectedFestivalId == 0 || festivalPermissions.All(p => p.Value != neededPolicy))
            NavigationManager.NavigateTo("/forbidden");
        Console.WriteLine("Check Permission 6");
        await Task.CompletedTask;
    }

    protected async Task LoadSelectedFestivalId()
    {
        if (await LocalStorageService.ContainKeyAsync(StorageConstants.Local.AdminSelectedFestivalId))
        {
            SelectedFestivalId =
                await LocalStorageService.GetItemAsync<int>(StorageConstants.Local.AdminSelectedFestivalId);
        }
        else
        {
            if (await LocalStorageService
                    .ContainKeyAsync(StorageConstants.Local.SelectedFestivalId))
            {
                var selectedFestivalId=
                    await LocalStorageService
                        .GetItemAsync<int?>(StorageConstants.Local.SelectedFestivalId);
                if (selectedFestivalId != null)
                {
                    SelectedFestivalId = selectedFestivalId.Value;
                }
                else
                {
                    await SetFestivalIdToSelectedFestivalId();
                }
            }
            else
            {
                await  SetFestivalIdToSelectedFestivalId();
            }

            FestivalId = await LocalStorageService
                .GetItemAsync<int>(StorageConstants.Local.FestivalId);
        }
    }

    private async Task SetFestivalIdToSelectedFestivalId()
    {
        SelectedFestivalId=await LocalStorageService
            .GetItemAsync<int>(StorageConstants.Local.FestivalId);
    }
}
