using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.Infrastructure.Managers.UsersAccounts;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.SpecialAccounts.Queries;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using Web.Extensions;
using Color = MudBlazor.Color;
using Severity = MudBlazor.Severity;

namespace Web.Components.Shared.Components;

public partial class UserCard
{
    [Inject] private SelectedAccountService SelectedAccountService { get; set; }

    [Inject] private IUserAccountManager AccountManager { get; set; }

    //[CascadingParameter]
    //private IHttpContextAccessor HttpContextAccessor { get; set; }
    #region Parameter

    [CascadingParameter] public HubConnection hubConnection { get; set; }
    [Parameter] public string Class { get; set; }
    [Parameter] public bool UserInHeader { get; set; }
    [Parameter] public string ImageDataUrl { get; set; }
    [Parameter] public bool ShowAnotherAccount { get; set; }
    [Parameter] public bool FromNavMenu { get; set; }

    #endregion


    private string FirstName { get; set; }
    private string SecondName { get; set; }
    private string Email { get; set; }
    private char FirstLetterOfName { get; set; }

    private int? _selectedFestivalId;

    private string SelectedFullName { get; set; }
    private string SelectedEmail { get; set; }
    private string SelectedImageUrl { get; set; }
    private bool _loaded;
    private bool _isGoldAccount;
    private string _goldDescription = "You are using a free account. Upgrade to Gold for more features.";
    private string? currentUrl;

    protected override async Task OnInitializedAsync()
    {
        currentUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        SelectedAccountService.AdminLoginToFestival += ChangeSelectedFestival;
        await LoadDataAsync();
        await LoadStatus();
        SelectedEmail = Email;
        SelectedImageUrl = ImageDataUrl;
        SelectedFullName = $"{FirstName} {SecondName}";
        _loaded = true;

        if (await AuthenticationManager.GetAdminLoginToFestivalId()!=null)
        {
            ChangeSelectedFestival(this,
               ((await AuthenticationManager.GetAdminLoginToFestivalId())!).Value);
        }
        else
        {
            //var fh =  (await AuthenticationManager.CurrentUser())
            //    .HasClaim(p=>p.Type==ClaimTypes.Role,).First(p=>p.cla RoleConstants.AdministratorRole);

            _selectedFestivalId =await AuthenticationManager.GetSelectedFestivalId();

            FestivalId =  AuthenticationManager.GetMainFestivalId()??0;

            if (FestivalId != 0)
            {
                await LoadFestivalData();
            }

            await LoadOtherFestival();
            if (_selectedFestivalId != null)
            {
                if (Festival != null && _selectedFestivalId == Festival.Id)
                {
                    SelectedEmail = Festival.Email;
                    SelectedFullName = Festival.Name;
                    SelectedImageUrl = Festival.LogoURL;
                }
                else
                {
                    var f = FestivalNames.FirstOrDefault(p => p.Id == _selectedFestivalId.Value);
                    if (f != null)
                    {
                        SelectedEmail = f.Email;
                        SelectedFullName = f.Name;
                        SelectedImageUrl = f.LogoURL;
                    }
                }
            }
        }

        await IsAdmin();
        await SetNotifUrl();
        await SetMessageUrl();
    }



    public GetFestivalNamesResponse _adminSelectFestival = new();


    private async Task LoadStatus()
    {
        var response = await AccountManager.GetAccountType(new GetUserAccountTypeQuery
        {

        });
        if (response.Succeeded)
        {
            _isGoldAccount = response.Data.FeeStatus==FeeStatus.Special;
            if (_isGoldAccount && response.Data.CloseDate.HasValue)
                _goldDescription = $"Gold Account ;Valid until {response.Data.CloseDate.Value.ToShortDateString()}";
        }
        else
        {
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
        }
    }
    private async void ChangeSelectedFestival(object sender, int festivalId)
    {
        var response = await FestivalManager.GetFestivalNames(new GetFestivalNamesQuery
        { FestivalIdString = festivalId.ToString() });

        if (response.Succeeded)
        {
            _adminSelectFestival = response.Data?.FirstOrDefault();
            if (_adminSelectFestival is null)
            {
                _snackBar.Add("The festival account could not be loaded.", Severity.Error);
                return;
            }

            FestivalNames.Add(new GetFestivalNamesResponse()
            {
                Id = festivalId,
                LogoURL = _adminSelectFestival.LogoURL,
                Name = _adminSelectFestival.Name,
                AdminLogin = true,
            });

            SelectedImageUrl = _adminSelectFestival.LogoURL;
            SelectedFullName = _adminSelectFestival.Name;
            SelectedEmail = _adminSelectFestival.Email;
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

    
        await InvokeAsync(StateHasChanged);
    }

    private void ChangeFestival(object sender, int? e)
    {
        throw new NotImplementedException();
    }

    private async Task LoadDataAsync()
    {
        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        Email = user.GetEmail().Replace(".com", string.Empty);
        FirstName = user.GetFirstName();
        SecondName = user.GetLastName();
        if (!string.IsNullOrWhiteSpace(FirstName))
        {
            FirstLetterOfName = FirstName[0];
        }

        var userId = user.GetUserId();
        var imageResponse = await _accountManager.GetProfilePictureAsync(userId);
        if (imageResponse.Succeeded && imageResponse != null)
        {
            ImageDataUrl = imageResponse.Data;
        }
    }

    [Inject] private IFestivalManager FestivalManager { get; set; }

    private bool _isFestivalUser;
    private List<GetFestivalNamesResponse> FestivalNames { get; set; } = new();
    private GetFestivalDetailResponse Festival;
    private int FestivalId { get; set; }
    private bool _isAdmin;

    private async Task LoadFestivalData()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            Festival = response.Data;
    }

    private async Task LoadOtherFestival()
    {
        var festivalIds = AuthenticationManager.GetOtherFestivalId();

        await GetFestivalNames(festivalIds.ToList());
    }


    private async Task GetFestivalNames(List<int> festivalIds)
    {
        var response = await FestivalManager.GetFestivalNames(new GetFestivalNamesQuery
        {
            FestivalIdString = string.Join(',', festivalIds)
        });

        if (response.Succeeded)
            FestivalNames = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private async Task SelectAccount(int? festivalId = null)
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AdminSelectedFestivalId);
        await SetSelectedAccount(festivalId);

        SelectedAccountService.SelectedAccountChanged(festivalId);
        NotificationService.ChangeNotificationBar();
        _navigationManager.NavigateTo(festivalId == null ? "/user/dashboard" : "/festival/dashboard");
    }

    private async Task Logout()
    {
        var currentUserId = (await AuthenticationManager.CurrentUser()).GetUserId();
        var parameters = new DialogParameters
        {
            { nameof(Dialogs.Logout.ContentText), "Logout Confirmation" },
            { nameof(Dialogs.Logout.ButtonText), "Logout" },
            { nameof(Dialogs.Logout.Color), Color.Error },
            { nameof(Dialogs.Logout.CurrentUserId), currentUserId },
            { nameof(Dialogs.Logout.HubConnection), hubConnection }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        _dialogService.Show<Dialogs.Logout>("Logout", parameters, options);
    }

    private async Task SetSelectedAccount(int? festivalId)
    {
        if ((await _localStorage.ContainKeyAsync(StorageConstants.Local.AdminSelectedFestivalId)))
        {
            ChangeSelectedFestival(this,
                await _localStorage.GetItemAsync<int>(StorageConstants.Local.AdminSelectedFestivalId));
        }
        else
        {
            _selectedFestivalId = festivalId;
            await _localStorage.SetItemAsync(StorageConstants.Local.SelectedFestivalId, festivalId);
            if (_selectedFestivalId != null)
            {
                if (Festival != null && _selectedFestivalId == Festival.Id)
                {
                    SelectedEmail = Festival.Email;
                    SelectedFullName = Festival.Name;
                    SelectedImageUrl = Festival.LogoURL;
                    await SetMessageUrl();
                    await SetNotifUrl();
                }
                else
                {
                    var f = FestivalNames.FirstOrDefault(p => p.Id == _selectedFestivalId.Value);
                    if (f != null)
                    {
                        SelectedEmail = f.Email;
                        SelectedFullName = f.Name;
                        SelectedImageUrl = f.LogoURL;
                    }
                }
            }
            else
            {
                SelectedEmail = Email;
                SelectedImageUrl = ImageDataUrl;
                SelectedFullName = $"{FirstName} {SecondName}";
            }
        }
    }

    private string notifUrl;
    private string messageUrl;

    private async Task SetNotifUrl()
    {
        if (_selectedFestivalId != null)
            notifUrl = "/festival/notifications";

        if (await IsAdmin())
            notifUrl = "/admin/notifications";
        else
            notifUrl = "/user/notifications";
    }

    private async Task SetMessageUrl()
    {
        if (_selectedFestivalId != null)
            messageUrl = "/festival/chat";

        if (await IsAdmin())
            messageUrl = "/admin/chat";
        else
            messageUrl = "/chat";
    }

    public async Task StateChanged(int? festivalId = null)
    {
        await SetSelectedAccount(festivalId);
        StateHasChanged();
    }

    private async Task<string> GetMessageUrl()
    {
        if (_selectedFestivalId != null)
            return "/festival/chat";
        if (await IsAdmin())
            return "/admin/chat";
        return "/chat";
    }

    private async Task<string> GetNotificationUrl()
    {
        if (_selectedFestivalId != null)
            return "/festival/notifications";
        if (await IsAdmin())
            return "/admin/notifications";
        return "/user/notifications";
    }

    private async Task<bool> IsAdmin()
    {
        _isAdmin = (await AuthenticationManager.CurrentUser())
            .IsInRole(RoleConstants.AdministratorRole);

        return _isAdmin;
    }
}

