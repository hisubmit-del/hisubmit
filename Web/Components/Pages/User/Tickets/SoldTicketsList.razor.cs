using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.SoldTickets;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System.IO;
using System.Threading;
using ClientComponents.Shared.Dialogs;
using Web.Components.Shared.Dialogs;

namespace Web.Components.Pages.User.Tickets;

public partial class SoldTicketsList
{
    [Inject] private ISoldTicketManager SoldTicketManager { get; set; }
    [Inject] private ILocalStorageService localStorageService { get; set; }

    private int FestivalId { get; set; }

    [Parameter] public int FestivalIdParam { get; set; }


    private List<GetAllSoldTicketResponse> _pagedDate { get; set; }
    public GetAllSoldTicketQuery Query { get; set; }


    private MudTable<GetAllSoldTicketResponse> _table;
    private GetAllSoldTicketQuery _advancedSearch { get; set; } = new GetAllSoldTicketQuery();
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
        await LoadUserId();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    private async Task LoadUserId()
    {
        CurrentUserId = (await AuthenticationManager.CurrentUser()).GetUserId();
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllSoldTicketResponse>> ServerReload(TableState state,System.Threading.CancellationToken token)
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
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.UserId = CurrentUserId;
        var response = await SoldTicketManager.GetAllSoldTicket(query);
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
                Console.WriteLine(message);
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

    private async Task DownloadFile(int id)
    {
        var response = await SoldTicketManager
            .DownloadTickets(new DownloadTicketsFileQuery()
            {
                SoldTicketId = id
            });
        if (response.Succeeded)
        {
            if (response.Data.File!=null && response.Data.File.Any())
            {
                var fileStream = new MemoryStream(response.Data.File);
                using var streamRef = new DotNetStreamReference(fileStream);
                await _jsRuntime.InvokeVoidAsync("downloadFileFromStream",
                    response.Data.FileName, streamRef);
            }
            else
            {
                _dialogService
                    .Show<InfoDialog>("Info", new DialogParameters
                    {
                        {
                            nameof(InfoDialog.Text),
                            "The ticket file is being prepared. Please try again in a few minutes"
                        }
                    }, new DialogOptions()
                    {
                        FullWidth = true,
                        MaxWidth = MaxWidth.Small
                    });
            }
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
