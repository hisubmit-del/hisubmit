using Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Identity.Roles;
using HiSubmit.Client.Infrastructure.Settings;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Contents;
using Web.Components.Shared.Components;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Constants.Storage;

namespace Web.Components.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] private IRoleManager RoleManager { get; set; }
        [Inject] private ILocalStorageService LocalStorageService { get; set; }
        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string SecondName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfName { get; set; }

        private bool _showCreateFestivalButton;

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;

            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
                FirstName = user.GetFirstName();
                if (FirstName.Length > 0)
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
                    _snackBar
                        .Add(Localize["You are logged out because the user with your Token has been deleted."],
                            Severity.Error);
                    await AuthenticationManager.Logout();
                }

                //await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);
            }
        }

        [Obsolete]
        private readonly MudTheme _currentTheme = BlazorHeroTheme.DefaultTheme;
        private bool _drawerOpen = true;
        private bool _rightToLeft = false;
        private int _festivalId = 0;


        private bool open;

        private void ToggleDrawer()
        {
            open = !open;
        }

        private async Task LoadFestivalId()
        {
            _festivalId =  AuthenticationManager.GetMainFestivalId()??0;
        }

        protected override async Task OnInitializedAsync()
        {
            var f = OperatingSystem.IsBrowser();
            if (f)
            {
                await _jsRuntime.InvokeVoidAsync("DeleteLoadingAnimation");
            }
           
            await LoadItems();
            await LoadDataAsync();
            // _currentTheme = BlazorHeroTheme.DefaultTheme;
            // _currentTheme = await _clientPreferenceManager.GetCurrentThemeAsync();
            _rightToLeft = await _clientPreferenceManager.IsRtl();
            _interceptor.RegisterEvent();
            await LoadFestivalId();
            var adminRole =
                (await AuthenticationManager.CurrentUser())
                .IsInRole(RoleConstants.AdministratorRole);

            if (_festivalId == 0 && !adminRole)
            {
                _showCreateFestivalButton = true;
            }

          
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                GalleryImagesOverallyService.OnOverlayImageChanged += (e, s) => StateHasChanged();

                _subscription = ApplicationState.RegisterOnPersisting(PersistMenu);
                _MainLayoutService.ChangedVisibleDrawer += this.DrawerVisibleToggle;
                _MainLayoutService.UserLoginedAccount += async (e, s) => await CheckCreatebuttonVisible(e, s);

                hubConnection = hubConnection.TryInitialize(_navigationManager);
                await hubConnection.StartAsync();
                await ConfigSignalR();
                hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification,
                    (message, receiverUserId, senderUserId) =>
                    {
                        if (CurrentUserId == receiverUserId)
                        {
                            _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                            _snackBar.Add(message, Severity.Info, config =>
                            {
                                config.VisibleStateDuration = 10000;
                                config.HideTransitionDuration = 500;
                                config.ShowTransitionDuration = 500;
                                config.Action = Localize["Chat?"];
                                config.ActionColor = Color.Primary;
                                config.OnClick = snackbar =>
                                {
                                    _navigationManager.NavigateTo($"chat/{senderUserId}");
                                    return Task.CompletedTask;
                                };
                            });
                        }
                    });

                hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
                {
                    try
                    {
                        var token = await AuthenticationManager.TryForceRefreshToken();
                        if (!string.IsNullOrEmpty(token))
                        {
                            _snackBar.Add(Localize["Refreshed Token."], Severity.Success);
                            _httpClient.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", token);
                        }
                    }
                    catch
                    {
                        _snackBar.Add(Localize["You are Logged Out by hub connection."], Severity.Error);
                        await AuthenticationManager.Logout();
                        _navigationManager.NavigateTo("/");
                    }
                });

                hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
                {
                    if (CurrentUserId != userId)
                    {
                        var rolesResponse = await RoleManager.GetRolesAsync();
                        if (rolesResponse.Succeeded)
                        {
                            var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
                            if (role != null)
                            {
                                var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
                                if (currentUserRolesResponse.Succeeded &&
                                    currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
                                {
                                    _snackBar.Add(
                                        Localize[
                                            "You are logged out because the Permissions of one of your Roles have been updated."],
                                        Severity.Error);
                                    await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
                                    await AuthenticationManager.Logout();
                                    _navigationManager.NavigateTo("/login");
                                }
                            }
                        }
                    }
                });
            }
        }

        private async Task CheckCreatebuttonVisible(object sender, EventArgs e)
        {
            _festivalId=AuthenticationManager.GetMainFestivalId()??0;

            var adminRole =
                (await AuthenticationManager.CurrentUser())
                .IsInRole(RoleConstants.AdministratorRole);

            _showCreateFestivalButton=_festivalId==0 && !adminRole;
        
            await InvokeAsync(StateHasChanged);
        }

        private Task ConfigSignalR()
        {
            hubConnection.On<string, int>(ApplicationConstants.SignalR.ReceiveMessageUserNotification,
                async (userId, roomId) =>
                {
                    if (CurrentUserId == userId)
                    {
                        await _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                        _snackBar.Add("You have a new message", Severity.Info, config =>
                        {
                            config.VisibleStateDuration = 10000;
                            config.HideTransitionDuration = 500;
                            config.ShowTransitionDuration = 500;
                            config.Action = Localize["Chat?"];
                            config.ActionColor = Color.Primary;
                            config.OnClick = snackbar =>
                            {
                                _navigationManager.NavigateTo($"newChat/{roomId}");
                                return Task.CompletedTask;
                            };
                        });
                    }
                });
            hubConnection.On<int>(ApplicationConstants.SignalR.ReceiveMessageAdminNotification,
                async (roomId) =>
                {
                    if ((await AuthenticationManager.CurrentUser()).IsInRole(RoleConstants.AdministratorRole))
                    {
                        await _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                        _snackBar.Add("You have a new message", Severity.Info, config =>
                        {
                            config.VisibleStateDuration = 10000;
                            config.HideTransitionDuration = 500;
                            config.ShowTransitionDuration = 500;
                            config.Action = Localize["Chat?"];
                            config.ActionColor = Color.Primary;
                            config.OnClick = snackbar =>
                            {
                                _navigationManager.NavigateTo($"/admin/newChat/{roomId}");
                                return Task.CompletedTask;
                            };
                        });
                    }
                });
            hubConnection.On<int, int>(ApplicationConstants.SignalR.ReceiveMessageFestivalNotification,
                async (festivalId, roomId) =>
                {
                    var fesId = await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);
                    if (festivalId == fesId)
                    {
                        await _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
                        _snackBar.Add("You have a new message", Severity.Info, config =>
                        {
                            config.VisibleStateDuration = 10000;
                            config.HideTransitionDuration = 500;
                            config.ShowTransitionDuration = 500;
                            config.Action = Localize["Chat?"];
                            config.ActionColor = Color.Primary;
                            config.OnClick = snackbar =>
                            {
                                _navigationManager.NavigateTo($"/festival/newChat/{roomId}");
                                return Task.CompletedTask;
                            };
                        });
                    }
                });
            return Task.CompletedTask;
        }

        private void DrawerOpenToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private bool _drawerOpen2 = false;
        private void DrawerOpenToggle2()
        {
            _drawerOpen2 = !_drawerOpen2;
        }
        private void DrawerVisibleToggle(object sender, bool showDrawer)
        {
            _showDrawer = showDrawer;
            StateHasChanged();
        }

        void IDisposable.Dispose()
        {
            _interceptor.DisposeEvent();
            //_ = hubConnection.DisposeAsync();
        }

        private async Task AddFestival()
        {
            var parameters = new DialogParameters();

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                
            };
            var dialog = _dialogService.Show<AddFestivalModal>
                (Localize["Add festival"], parameters, options);
            var result = await dialog.Result;
        }

        private HubConnection hubConnection;
        public bool IsConnected => hubConnection.State == HubConnectionState.Connected;


        [Inject] private IContentManager ContentManager { get; set; }

        private List<FooterItemDto> _menuItems = new();
        private bool _showDrawer;

        private async Task LoadItems()
        {
            if (ApplicationState.TryTakeFromJson
                    <List<FooterItemDto>>("menuitem", out var stored))
            {
                _menuItems = stored;
            }
            else
            {
                var response = await ContentManager.GetFooterItems
                    (new GetAllFooterItemQuery());
                if (response.Succeeded)
                {
                    _menuItems = response.Data;
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

        #region Prerendering

        private PersistingComponentStateSubscription _subscription;

        private Task PersistMenu()
        {
            ApplicationState.PersistAsJson("menuitem", _menuItems);
            return Task.CompletedTask;
        }

        #endregion

        // private bool GetDrawerShow()
        // {
        //     var publicUrl=
        // }
    }
}
