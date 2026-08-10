using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using Hisubmit.Client.SharedModels.Features.Wrapper;
using HiSubmit.Client.Infrastructure.Managers.FestivalProductsSold;
using HiSubmit.Client.Infrastructure.Managers.ProductsSold;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.User.Products;

public partial class ProductsSoldList
{
     #region Inject

    [Inject] private IProductSoldManager SoldProductManager { get; set; }

    #endregion
    
    private List<GetAllSoldProductResponse> _pagedDate { get; set; }
    public GetAllSoldTicketQuery Query { get; set; }


    private MudTable<GetAllSoldProductResponse> _table;
    private GetAllSoldProductQuery _advancedSearch { get; set; } = new ();
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
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.User,
            NotificationTypes = NotificationType.FestivalSoldTicket
        });
        NotificationService.ChangeNotificationBar();
    }
    
    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllSoldProductResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllSoldProductQuery()
        {
            RequestAccountType = RequestAccountType.Festival
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
        return new TableData<GetAllSoldProductResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllSoldProductQuery query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        var response = await SoldProductManager.GetAllAsync(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.ProductName != null &&
                    element.ProductName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.ProductName);
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

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private void ShowDetail(int productSoldId)
    {
        var parameters = new DialogParameters
        {
            {nameof(ProductSoldDetailDialog.ProductSoldId),productSoldId}
        };
        var options = new DialogOptions
        {
            CloseButton = true,
           
            FullWidth = true,
            MaxWidth = MaxWidth.Medium
        };
        _dialogService.Show<ProductSoldDetailDialog>("Product Sold Detail", parameters, options);
    }
}