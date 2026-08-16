using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Advertise;

[Authorize(Policy=Permissions.Advertise.RequestView)]
public partial class Advertises
{
    #region Injection

    [Inject] public IAdminAdvertiseManager FestivalManager { get; set; }

    #endregion
    
    private int FestivalId { get; set; }

    private List<GetAllAdvertiseResponse> _pagedDate { get; set; }
    public GetAllAdvertiseRequest Request { get; set; }

    private MudTable<GetAllAdvertiseResponse> _table;
    private GetAllAdvertiseRequest _advancedSearch { get; set; } = new();
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

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await SeenNotification();
                
        await base.OnAfterRenderAsync(firstRender);
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllAdvertiseResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllAdvertiseRequest();
        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllAdvertiseResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllAdvertiseRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;

        var response = await FestivalManager.GetAllAsync(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (element.Email != null &&
                    element.Email.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                // if (element.Name != null &&
                //     element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            });
            switch (state.SortLabel)
            {
                case "Title":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Email);
                    break;
                // case "SubmitDateFrom":
                //     loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.SubmitDateFrom);
                //     break;
            }

            data = loadedData.ToList();
            _pagedDate = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Info);
            }
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async  Task Detail(int id)
    {
        var parameters = new DialogParameters
        {
            {nameof(AdvertiseRequestDetail.RequestId),id}
        };
        var options = new DialogOptions()
        {
            Position = DialogPosition.Center,
            CloseButton = true,
           
            FullWidth = true,
            MaxWidth = MaxWidth.Medium
        };
        _dialogService.Show<AdvertiseRequestDetail>("Filter Detail", parameters, options);
    }

    private async Task SeenNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminAdvertiseRequest,
        }); 
        NotificationService.ChangeNotificationBar();
    }
}