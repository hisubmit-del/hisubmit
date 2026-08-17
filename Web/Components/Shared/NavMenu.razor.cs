using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using HiSubmit.Client.Infrastructure.Services;
using Web.Components.Shared.Components;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Shared;

public partial class NavMenu
{
    #region Parameters

    [Parameter] public bool AccountPages { get; set; } = true;

    [Parameter] public bool ShowCreateFestivalButton { get; set; }

    #endregion

    #region Injects

    [Inject] private SelectedAccountService SelectedAccountService { get; set; }

    [Inject] private IContentManager ContentManager { get; set; }
    //[Inject] private HiSubmitAuthenticationStateProvider _stateProvider { get; set; }
    #endregion

    [Parameter] public List<FooterItemDto> MenuItems { get; set; } = new();


    private ClaimsPrincipal _authenticationStateProviderUser;

    private int? SelectedFestivalId { get; set; }
    private int FestivalId { get; set; }
    private UserCard _userCardComponent;
    private bool _adminSelectedFestival;

    protected override async Task OnInitializedAsync()
    {
        _subscription = ApplicationState.RegisterOnPersisting(PersistMenu);
        _adminSelectedFestival = await AdminSelectFestival();
        await LoadMenuItems();
        await base.OnInitializedAsync();
    }

    private async Task LoadMenuItems()
    {
        if (ApplicationState.TryTakeFromJson
                <List<FooterItemDto>>("menuitemNavMenu", out var stored))
        {
            MenuItems = stored;
        }
        else
        {
            var response = await ContentManager.GetFooterItems(new GetAllFooterItemQuery());
            if (response.Succeeded)
                MenuItems = response.Data.Where(p => p.Position == MenuItemPosition.Header)
                    .ToList();
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
    }
    protected override async Task OnParametersSetAsync() 
    {
        SelectedAccountService.SelectedAccount +=
            async (s, h) => await OnChangeNSelectedAccount(s, h);

        SelectedAccountService.AdminLoginToFestival +=async (s, h) => await ReloadComponents(s, h);

        var user = await AuthenticationManager.CurrentUser();
        //_authenticationStateProviderUser =
            //await _stateProvider.GetAuthenticationStateProviderUserAsync();
            SelectedFestivalId = await AuthenticationManager.GetSelectedFestivalId();

            //if (sId)
            //{
            //    var findFirst = user.FindFirst(ApplicationClaimTypes.SelectedFestival);
            //    if (int.TryParse(findFirst?.Value, out var s))
            //    {
            //        SelectedFestivalId = s;
            //    }
            //}
            //else
            //{
            //    SelectedFestivalId = null;
            //}


        //  await _localStorage.GetItemAsync<int?>(StorageConstants.Local.SelectedFestivalId);

        if (user.HasClaim(p => p.Type == ApplicationClaimTypes.FestivalId))
        {
            var findFirst = user.FindFirst(ApplicationClaimTypes.FestivalId);
            if (int.TryParse(findFirst?.Value, out var s))
            {
                FestivalId = s;
            }
        }
        //else
        //{
        //    SelectedFestivalId = null;
        //}
        //FestivalId =


        //    await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);


        var currentUser = await AuthenticationManager.CurrentUser();
        IsAdminRole = currentUser.IsInRole(RoleConstants.AdministratorRole);
        IsFestivalRole = currentUser.IsInRole(RoleConstants.FestivalRole);
    }

    private async Task ReloadComponents(object sender, int festivalId)
    {
        await _userCardComponent.StateChanged(festivalId);
        StateHasChanged();
    }

    private async Task OnChangeNSelectedAccount(object sender, int? festivalId)
    {

        if (await AuthenticationManager.GetAdminLoginToFestivalId() !=null)
        {
            SelectedFestivalId=await AuthenticationManager.GetAdminLoginToFestivalId() ??0;
        }

        else
        {
            SelectedFestivalId =await AuthenticationManager.GetSelectedFestivalId();
                
                //await _localStorage
                //.GetItemAsync<int?>(StorageConstants.Local.SelectedFestivalId);
            IsAdminRole = (await AuthenticationManager.CurrentUser())
                .IsInRole(RoleConstants.AdministratorRole);
            IsFestivalRole = (await AuthenticationManager.CurrentUser())
                .IsInRole(RoleConstants.FestivalRole);

        }
        _adminSelectedFestival = await AdminSelectFestival();
        await _userCardComponent.StateChanged(festivalId);
        StateHasChanged();
    }

    private bool IsAdminRole { get; set; }
    private bool IsFestivalRole { get; set; }

    private void ToggleMenuItem()
    {
        AccountPages = !AccountPages;
    }

    private async Task AddFestival()
    {
        var parameters = new DialogParameters();

        var options = new DialogOptions
        { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<AddFestivalModal>(Localize["Add festival"], parameters, options);
        var result = await dialog.Result;
    }

    #region Prerendering

    private PersistingComponentStateSubscription _subscription;

    private Task PersistMenu()
    {
        ApplicationState.PersistAsJson("menuitemNavMenu", MenuItems);
        return Task.CompletedTask;
    }

    #endregion

    private async Task<bool> AdminSelectFestival()
    {
        var f = false;

        f = await AuthenticationManager.GetAdminLoginToFestivalId() !=null;

        return f;
    }
}
