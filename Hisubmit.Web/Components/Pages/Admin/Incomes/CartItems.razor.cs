using System;
using MudBlazor;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using HiSubmit.Client.Infrastructure.Managers.MasterFestivals;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Hisubmit.Client.SharedModels.Features.MasterFestivals.Queries;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Web.Components.Pages.Admin.Incomes;

public partial class CartItems
{
    #region Injects

    [Inject] private IAdminPaymentManager FestivalPaymentsManager { get; set; }
    [Inject] private IAdminFestivalManager AdminFestivalManager { get; set; }
    [Inject] private IMasterFestivalManager MasterFestivalManagerManager { get; set; }

    #endregion

    private List<GetCartItemResponse> _pagedDate { get; set; } = new();
    public PaymentFilterDto Filter { get; set; } = new();

    private GetAllMasterFestivalResponse MasterFestival { get; set; } = null;
    private List<GetFestivalNamesResponse> FestivalNames { get; set; } = new();

    private MudTable<GetCartItemResponse> _table;
    private int _totalItems;
    private string _searchString = "";
    private bool _openSearchForm;
    private bool _loaded;
    private bool _loadedFestivalId;


    private List<GetAllMasterFestivalResponse> Festivals { get; set; } = new();
    private int? _selectedFestivalId;

    protected override async Task OnInitializedAsync()

    {
        await LoadFestival();
        await base.OnInitializedAsync();
    }

    private async Task<TableData<GetCartItemResponse>>
        ServerReload(TableState state, CancellationToken token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        //var request = new GetAllCartItemQuery()
        //{
        //    FestivalId = _selectedFestivalId
        //};
        //if (IsAdvancedSearch)
        //{
        //    request = _advancedSearch;
        //}
        //else
        //{
        //    request.SearchString = _searchString;
        //}

        await LoadData(state.Page, state.PageSize, state, Filter);
        _loaded = true;
        return new TableData<GetCartItemResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, PaymentFilterDto query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        //request.ItemType = GetCartItemQueryType.Admin;

        var response = await FestivalPaymentsManager.GetAllCartItemAsync(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            // _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.Title != null &&
                    element.Title.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                // if (element.Name != null &&
                //     element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            });
            switch (state.SortLabel)
            {
                case "Title":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Title);
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

    private async Task Search()
    {
        await _table.ReloadServerData();
    }

    private async Task ToggleSearch()
    {
        _openSearchForm = !_openSearchForm;
        if (!_openSearchForm)
        {
            Filter = new PaymentFilterDto();
            await _table.ReloadServerData();
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task LoadFestival()
    {
        var response =
            await MasterFestivalManagerManager.GetAll(new GetAllMasterFestivalRequest());
        if (response.Succeeded)
        {
            Festivals = response.Data;
            FestivalNames = Festivals.SelectMany(p => p.Festivals).ToList();
        }
    }

    private  async Task ChangedFestivalList(int? item)
    {
        MasterFestival = null;
        Filter.FestivalId = null;
        await InvokeAsync(StateHasChanged);
        Filter.MasterFestivalId = item;
        
        MasterFestival = 
            item == null ? null : Festivals.FirstOrDefault(p => p.Id == item);
        
        await InvokeAsync(StateHasChanged);
    }
}