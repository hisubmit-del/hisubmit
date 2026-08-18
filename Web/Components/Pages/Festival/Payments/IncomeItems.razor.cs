using MudBlazor;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Client.Infrastructure.Managers.FestivalPayments;

namespace Web.Components.Pages.Festival.Payments;

public partial class IncomeItems
{
    #region Injects
    [Inject]
    private IFestivalPaymentsManager FestivalPaymentsManager { get; set; }
    #endregion

    #region Parameters

    [Parameter]
    public  int FestivalId { get; set; }

    #endregion
    private List<GetCartItemResponse> _pagedDate { get; set; } = new();
    public GetAllCartItemQuery Query { get; set; }


    private MudTable<GetCartItemResponse> _table;
    private GetAllCartItemQuery _advancedSearch  = new ();
    private int _totalItems;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool _loaded;
    protected override async Task OnInitializedAsync()

    {
        //await base.CheckPermission(Permissions.FestivalPayment.CartItem);
       // _loadedFestivalId = true;
        await base.OnInitializedAsync();
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
    }

    private async Task<TableData<GetCartItemResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllCartItemQuery
        {
            FestivalId = FestivalId
        };
        //if (IsAdvancedSearch)
        //{
        //    query = _advancedSearch;
        //}
        //else
        //{
            query.SearchString = _searchString;
        //}
        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetCartItemResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        //IsAdvancedSearch = true;
        await _table.ReloadServerData();
        //IsAdvancedSearch = false;
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllCartItemQuery query)
    {
        //await LoadFestivalId();
        //  await LoadFestivalId();
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.Type = GetCartItemQueryType.Festival;
        query.FestivalId = FestivalId;
        var response = await FestivalPaymentsManager.GetAll(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            //_currentPage = response.CurrentPage;
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

    private string GetTitle(GetCartItemResponse cartItem)
    {
        var title = string.Empty;
        switch (cartItem.CartItemType)
        {
            case CartItemType.Submit:
                title = cartItem.ProjectName;
                break;
            case CartItemType.Badge:
                title = cartItem.ProductName;
                break;
            case CartItemType.Ticket:
                title = cartItem.SoldTicketName;
                break;
            case CartItemType.Product:
                title = cartItem.ProductName;
                break;
            default:
                title = string.Empty;
                break;
        }
        return title;
    }
}
