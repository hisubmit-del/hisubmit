using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Web.Components.Pages.Public.Content;

public partial class Store
{
    [Inject] private IPublicFestivalManager FestivalManager { get; set; }

    private PaginatedResult<GetAllPagedProductsResponse> _response =
        new(new List<GetAllPagedProductsResponse>());
    private List<GetAllPagedProductsResponse> _products = new();
    private string _searchString;
    private int _pageNumber = 1;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await LoadProducts();
        _loaded = true;
    }

    private async Task LoadProducts()
    {
        var response = await FestivalManager.GetAllProducts(new GetAllProductsRequest
        {
            PageNumber = _pageNumber,
            PageSize = 16,
            SearchString = _searchString,
            IsEnable = true
        });

        if (response.Succeeded)
        {
            _response = response;
            _products = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task Search(KeyboardEventArgs _)
    {
        _pageNumber = 1;
        await LoadProducts();
    }

    private async Task ChangePage(int pageNumber)
    {
        _pageNumber = pageNumber;
        await LoadProducts();
    }
}

