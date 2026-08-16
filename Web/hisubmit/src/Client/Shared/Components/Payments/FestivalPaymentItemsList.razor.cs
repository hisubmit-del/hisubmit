using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Commands.Add;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Shared.Components.Payments;

public partial class FestivalPaymentItemsList
{
    #region Injects

    [Inject] private IAdminPaymentManager AdminPaymentManager { get; set; }
    [Inject] private IFestivalPaymentsManager FestivalPaymentsManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public bool IsAdmin { get; set; }
    [Parameter] public int? FestivalId { get; set; }
    [Parameter] public EventCallback OnChangeData { get; set; }
    #endregion

    private List<GetAllFestivalPaymentItemResponse> PagedDate { get; set; }
    public GetAllFestivalPaymentItemQuery Query { get; set; }


    private MudTable<GetAllFestivalPaymentItemResponse> _table;
    private readonly GetAllFestivalPaymentItemQuery _advancedSearch = new();
    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool _isAdvancedSearch = false;
    private string _displaySearchFrom = "d-none";
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllFestivalPaymentItemResponse>> 
        ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllFestivalPaymentItemQuery();
        if (_isAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllFestivalPaymentItemResponse> { TotalItems = _totalItems, Items = PagedDate };
    }

    private async Task AdvancedSearch()
    {
        _isAdvancedSearch = true;
        await _table.ReloadServerData();
        _isAdvancedSearch = false;
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllFestivalPaymentItemQuery query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = FestivalId;

        //request.FestivalId = FestivalId;

        PaginatedResult<GetAllFestivalPaymentItemResponse> response;
        if (IsAdmin)
        {
            response = await AdminPaymentManager.GetAllFestivalPaymentItem(query);
        }
        else
        {
            response = await FestivalPaymentsManager.GetAllFestivalPaymentItem(query);
        }

        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.FestivalName != null &&
                    element.FestivalName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return true;
            });
            switch (state.SortLabel)
            {
                case "Name":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.FestivalName);
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

    private async  Task Add()
    {
        if (FestivalId == null)
            return;
        var parameters = new DialogParameters()
        {
            {
                nameof(AddFestivalPaymentItemDialog.Item), new AddFestivalPaymentItemCommand
                {
                    FestivalId = FestivalId.Value,
                }
            }
        };
        var options = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            
        };
        var res=_dialogService.Show<AddFestivalPaymentItemDialog>("Add Item", parameters, options);
        if ((await res.Result) != DialogResult.Cancel())
        {
            await _table.ReloadServerData();
            await OnChangeData.InvokeAsync();
        }
    }
}