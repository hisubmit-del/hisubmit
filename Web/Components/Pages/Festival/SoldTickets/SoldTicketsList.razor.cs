using System;
using MudBlazor;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Web.Components.Shared.Components.Base;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using HiSubmit.Client.Infrastructure.Managers.SoldTickets;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;

namespace Web.Components.Pages.Festival.SoldTickets;

public partial class SoldTicketsList:BaseFestival
{
    #region Inject

    [Inject] private IFestivalSoldTicketManager FestivalSoldTicketManager { get; set; }

    #endregion
    
    private List<GetAllSoldTicketResponse> _pagedDate { get; set; }
    public GetAllSoldTicketQuery Query { get; set; }


    private MudTable<GetAllSoldTicketResponse> _table;
    private GetAllSoldTicketQuery _advancedSearch { get; set; } = new GetAllSoldTicketQuery();
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
        await CheckPermission(Permissions.SoldTickets.View);
        await base.OnInitializedAsync();
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            FestivalId = SelectedFestivalId,
            AccountType = SiteAccountType.Festival,
            NotificationTypes = NotificationType.FestivalSoldTicket
        });
        NotificationService.ChangeNotificationBar();
    }
    
    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllSoldTicketResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllSoldTicketQuery
        {
            SoldTicketStatus = SoldTicketStatus.Paid
        };

        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        return new TableData<GetAllSoldTicketResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllSoldTicketQuery query)
    {
        await LoadSelectedFestivalId();
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = SelectedFestivalId;
        var response = await FestivalSoldTicketManager.GetAllSoldTicket(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.TicketTitle != null &&
                    element.TicketTitle.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.TicketTitle);
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
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task OnSearch(string text)
    {
        _searchString = text?.Trim() ?? string.Empty;
        if (_table is not null)
            await _table.ReloadServerData();
    }
}
