using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.Enable;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Client.Infrastructure.Managers.AdminTickets;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Tickets;

public partial class Tickets
{
    [Inject] public IAdminTicketsManager TicketManager { get; set; }
    private List<GetAllTicketResponse> PagedDate { get; set; }
    public GetAllTicketQuery Query { get; set; }


    [Parameter]
    public  int? FestivalId { get; set; }
    
    private MudTable<GetAllTicketResponse> _table;
    private GetAllTicketQuery _advancedSearch { get; set; } = new GetAllTicketQuery();
    private string CurrentUserId { get; set; }
    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool IsAdvancedSearch = false;
    private string _displaySearchFrom = "d-none";
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _loaded = true;
        await SeenNotification();
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllTicketResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllTicketQuery();
        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        return new TableData<GetAllTicketResponse> { TotalItems = _totalItems, Items = PagedDate };
    }

    private async Task SeenNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            NotificationTypes = NotificationType.AdminNewAddedTicketOrBadge,
            FestivalId = FestivalId,
            AccountType = SiteAccountType.Admin,
        });
        NotificationService.ChangeNotificationBar();
    }
    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllTicketQuery query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = FestivalId;
        var response = await TicketManager.GetAll(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.Title != null &&
                    element.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.VenueName != null &&
                    element.VenueName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Title);
                    break;
                // case "SubmitDateFrom":
                //     loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.SubmitDateFrom);
                //     break;
            }

            data = loadedData.ToList();
            PagedDate = data;
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

    private async Task Reset()
    {
        PagedDate = new List<GetAllTicketResponse>();
        await _table.ReloadServerData();
    }

    private async Task ChangeChecked(int contextId, bool b)
    {
        var response = await TicketManager.EnableTickets(new EnableTicketCommand()
        {
            IsEnable = b,
            TicketId = contextId
        });
        if (response.Succeeded)
            await _table.ReloadServerData();
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message);
            }
        }
    }
}