using Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Identity.Roles;
using HiSubmit.Client.Infrastructure.Settings;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Contents;

namespace Web.Components.Shared;

[AllowAnonymous]
public partial class PublicMainLayout
{
    [Inject] private IRoleManager RoleManager { get; set; }

    private string CurrentUserId { get; set; }
    private string ImageDataUrl { get; set; }
    private string FirstName { get; set; }
    private string SecondName { get; set; }
    private string Email { get; set; }
    private char FirstLetterOfName { get; set; }

    private async Task LoadDataAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user == null) return;
        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUserId = user.GetUserId();
            FirstName = user.GetFirstName();
            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                FirstLetterOfName = FirstName[0];
            }

            SecondName = user.GetLastName();
            Email = user.GetEmail();
            var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
            if (imageResponse.Succeeded)
            {
                ImageDataUrl = imageResponse.Data;
            }

            var currentUserResult = await _userManager.GetAsync(CurrentUserId);
            if (!currentUserResult.Succeeded || currentUserResult.Data == null)
            {
                _snackBar.Add(Localize["You are logged out because the user with your Token has been deleted."], Severity.Error);
                await AuthenticationManager.Logout();
            }

            if (hubConnection.State == HubConnectionState.Connected)
            {
                await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);
            }
        }
    }
    [Inject]
    public  IJSRuntime JsRuntime { get; set; }

    private MudTheme _currentTheme;
    private bool _drawerOpen = true;
    private bool _rightToLeft = false;
    private async Task RightToLeftToggle()
    {
        var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
        _rightToLeft = isRtl;
        _drawerOpen = false;
    }

    protected override async Task OnInitializedAsync()
    {
        hubConnection = hubConnection.TryInitialize(_navigationManager);
        await hubConnection.StartAsync();
        await LoadDataAsync();
        await LoadItems();

        _currentTheme = BlazorHeroTheme.DefaultTheme;
        _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
        _rightToLeft = await _clientPreferenceManager.IsRtl();
        _interceptor.RegisterEvent();
   
        //hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
        //{
        //    if (CurrentUserId == receiverUserId)
        //    {
        //        _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
        //        _snackBar.Add(message, Severity.Info, config =>
        //        {
        //            config.VisibleStateDuration = 10000;
        //            config.HideTransitionDuration = 500;
        //            config.ShowTransitionDuration = 500;
        //            config.Action = localize["Chat?"];
        //            config.ActionColor = Color.Primary;
        //            config.Onclick = snackbar =>
        //            {
        //                _navigationManager.NavigateTo($"chat/{senderUserId}");
        //                return Task.CompletedTask;
        //            };
        //        });
        //    }
        //});
        //hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
        //{
        //    try
        //    {
        //        var token = await _authenticationManager.TryForceRefreshToken();
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            _snackBar.Add(localize["Refreshed Token."], Severity.Success);
        //            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        _snackBar.Add(localize["You are Logged Out."], Severity.Error);
        //        await _authenticationManager.Logout();
        //        _navigationManager.NavigateTo("/");
        //    }
        //});
        //hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
        //{
        //    if (CurrentUserId != userId)
        //    {
        //        var rolesResponse = await RoleManager.GetRolesAsync();
        //        if (rolesResponse.Succeeded)
        //        {
        //            var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
        //            if (role != null)
        //            {
        //                var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
        //                if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
        //                {
        //                    _snackBar.Add(localize["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
        //                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
        //                    await _authenticationManager.Logout();
        //                    _navigationManager.NavigateTo("/login");
        //                }
        //            }
        //        }
        //    }
        //});
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await _jsRuntime.InvokeVoidAsync("mainInter");
        }

        await base.OnAfterRenderAsync(firstRender);
    }


    private bool open;

    private void ToggleDrawer()
    {
        open = !open;
    }
        
    [Inject]
    private  IContentManager ContentManager { get; set; }

    private List<FooterItemDto> _menuItemDto=new();

    private async Task LoadItems()
    {
        var response = await ContentManager.GetFooterItems(new GetAllFooterItemQuery());
        if (response.Succeeded)
        {
            _menuItemDto = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    private void Logout()
    {
        //var parameters = new DialogParameters
        //{
        //    {nameof(Dialogs.Logout.ContentText), $"{Localize["Logout Confirmation"]}"},
        //    {nameof(Dialogs.Logout.ButtonText), $"{Localize["Logout"]}"},
        //    {nameof(Dialogs.Logout.Color), Color.Error},
        //    {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
        //    {nameof(Dialogs.Logout.HubConnection), hubConnection}
        //};

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        //_dialogService.Show<Dialogs.Logout>(Localize["Logout"], parameters, options);
    }

    private void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }


    private void GoToLoginPage()
    {
        _navigationManager.NavigateTo("/Account/Login", forceLoad: true);
    }
    private async Task DarkMode()
    {
        var isDarkMode = await _clientPreferenceManager.ToggleDarkModeAsync();
        _currentTheme = isDarkMode
            ? BlazorHeroTheme.DefaultTheme
            : BlazorHeroTheme.DefaultTheme;
    }

    public void Dispose()
    {
        _interceptor.DisposeEvent();
        //_ = hubConnection.DisposeAsync();
    }

    private HubConnection hubConnection;
    public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
}
