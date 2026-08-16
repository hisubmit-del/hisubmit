using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Client.Infrastructure.Managers.PublicTicket;
using Web.Components.Shared.Components;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Public.Festivals.Tickets;

public partial class Tickets
{
    [Inject] private IPublicTicketManager TicketManager { get; set; }

    [Parameter] public int FestivalId { get; set; }

    private List<GetAllTicketResponse> _tickets = new();

    private List<GetAllTicketResponse> tickets { get; set; } = new();
    public GetAllTicketQuery Query { get; set; }


    private MudTable<GetAllTicketResponse> _table;
    private GetAllTicketQuery _advancedSearch { get; set; } = new ();
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
        await LoadTickets();
        await base.OnInitializedAsync();
        _loaded = true;
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    
    private async Task LoadTickets()
    {
        var response = await TicketManager.GetAllAsync(new GetAllTicketQuery()
        {
            FestivalId = FestivalId,
            GetAllData = true,
            GetActiveTicket = true
        });
        if (response.Succeeded)
        {
            tickets = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
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
        return new TableData<GetAllTicketResponse> { TotalItems = _totalItems, Items = tickets };
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
        var response = await TicketManager.GetAllAsync(query);
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
            }

            data = loadedData.ToList();
            tickets = data;
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
        tickets = new List<GetAllTicketResponse>();
        await _table.ReloadServerData();
    }

}