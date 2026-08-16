using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalState;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using ClientComponents.Shared.Components.Payments;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.Infrastructure.Services;

namespace ClientComponents.Pages.Admin.Festivals;

public partial class FestivalList
{
    [Inject] private IAdminFestivalManager AdminFestivalManager { get; set; }
    [Inject] private IAdminPaymentManager PaymentManager { get; set; }
    [Inject] private SelectedAccountService SelectedAccountService { get; set; }

    private IEnumerable<GetAllFestivalResponse> _pagedData;
    private MudTable<GetAllFestivalResponse> _table;
    private GetAllFestivalRequest _advancedSearch { get; set; } = new();
    private string CurrentUserId { get; set; }
    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _dense = false;
    private bool _striped = true;
    private bool _bordered = false;
    private bool _openSearchForm = false;
    private bool IsAdvancedSearch = false;
    private string _displaySearchFrom = "d-none";
    private bool _loaded;
    private bool _paymentInformationProcessing;
    private DateTime? OpeningDateFrom { get; set; }
    private DateTime? OpeningDateTo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _loaded = true;

        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user == null) return;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SeenFestivalNotification();

        await base.OnAfterRenderAsync(firstRender);
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllFestivalResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllFestivalRequest();
        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        return new TableData<GetAllFestivalResponse> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllFestivalRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;

        var response = await AdminFestivalManager.GetAllAsync(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(request.SearchString))
                    return true;
                if (element.Name != null &&
                    element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.UserName != null &&
                    element.UserName.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "FestivalOpeningDateField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.OpeningDate);
                    break;
                case "FestivalNameField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Name);
                    break;
                case "FestivalUserNameField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.UserName);
                    break;
            }

            data = loadedData.ToList();
            _pagedData = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task UpdateState(int festivalId, bool isActive)
    {
        var response = await AdminFestivalManager.UpdateStateAsync(new UpdateFestivalStateRequest()
        {
            Id = festivalId,
            IsActive = !isActive
        });
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            _pagedData.FirstOrDefault(p => p.Id == festivalId)!.IsActive = !isActive;
            await _table.ReloadServerData();
            StateHasChanged();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private void FestivalDetail(int festivalId)
    {
        _navigationManager.NavigateTo($"/Admin/festival/Detail/{festivalId}");
    }

    private async Task ConfirmSpecial(int id)
    {
        await UpdateFeeStatus(id, FeeStatus.Special);
    }

    private async Task NotConfirmSpecial(int id)
    {
        await UpdateFeeStatus(id, FeeStatus.Rejected);
    }

    private async Task UpdateFeeStatus(int id, FeeStatus feeStatus)
    {
        var response = await AdminFestivalManager.UpdateFeeStatus(new UpdateFestivalFeeStatusRequest()
        {
            FestivalId = id,
            FeeStatus = feeStatus
        });
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            await _table.ReloadServerData();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task SeenFestivalNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminNewFestival,
        });
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminReleaseFestivalRequest,
        });
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminSpecialFestivalRequest,
        });

        NotificationService.ChangeNotificationBar();
    }

    private async Task ShowPaymentInformation(int id)
    {
        _paymentInformationProcessing = true;
        var info = await PaymentManager.GetFestivalPaymentInformationPaymentAsync(
            new GetFestivalPaymentInformationDetailQuery
            {
                FestivalId = id
            });
        _paymentInformationProcessing = false;
        if (info.Succeeded)
        {
            var parameters = new DialogParameters
            {
                { nameof(FestivalPaymentInformationModal.Information), info.Data }
            };
            var options = new DialogOptions
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Medium,
                
            };
            _dialogService.Show(typeof(FestivalPaymentInformationModal), "Payment Information", parameters, options);
        }
    }

    private void GoToPaymentDetail(int id)
    {
        _navigationManager.NavigateTo($"/Admin/festival/payment/{id}");
    }

    private void GotoProducts(int id)
    {
        _navigationManager.NavigateTo($"/admin/festival/products/{id}");
    }

    private void GotoTickets(int id)
    {
        _navigationManager.NavigateTo($"/admin/festival/tickets/{id}");
    }

    private void GoToNews(int contextId)
    {
        _navigationManager.NavigateTo($"/admin/news/{contextId}");
    }

    private async Task LoginWithFestival(GetAllFestivalResponse festival)
    {
        await _localStorage.SetItemAsync(StorageConstants.Local.AdminSelectedFestivalId, festival.Id);
        SelectedAccountService.SelectedAccountChanged(festival.Id);
        SelectedAccountService.AdminLoginedToFestival(festival.Id);
        _navigationManager.NavigateTo($"/festival/dashboard");
    }
}