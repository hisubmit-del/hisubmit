using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.AdminFestival;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using HiSubmit.Client.Infrastructure.Services;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Features.Payments.Commands;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Web.Components.Shared.Dialogs;

namespace Web.Components.Pages.User.ShoppingCart;

public partial class Carts
{
     #region Injects

    [Inject] private ICartManager CartManager { get; set; }
  
    [Inject]public UserCartService UserCartService { get; set; }

    #endregion

    private List<GetAllCartsResponse> _pagedDate { get; set; } = new();
    public GetAllCartsFilterDto Filter { get; set; } = new();

    private bool _downloadProcess;
    private MudTable<GetAllCartsResponse> _table;
    private int _totalItems;
    private string _searchString = "";
    private bool _openSearchForm;
    private bool _loaded;
    private bool _loadedFestivalId;


    private List<GetAllFestivalResponse> Festivals { get; set; } = new();
    private int? _selectedFestivalId;

    protected override async Task OnInitializedAsync()

    {
       // await LoadFestival();
        await base.OnInitializedAsync();
    }

    private async Task<TableData<GetAllCartsResponse>>
        ServerReload(TableState state, CancellationToken token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }
        await LoadData(state.Page, state.PageSize, state, Filter);
        _loaded = true;
        return new TableData<GetAllCartsResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllCartsFilterDto query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        var response = await CartManager.GetAll(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            var data = response.Data;
            _pagedDate = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
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
            Filter = new GetAllCartsFilterDto();
            await _table.ReloadServerData();
        }
    }

    private async Task OnSearch(string text)
    {
        _searchString = text?.Trim() ?? string.Empty;
        Filter.SearchString = _searchString;
        if (_table is not null)
            await _table.ReloadServerData();
    }
    
    private async Task DownloadFactor(int id)
    {
        _downloadProcess = true;
        var response = await CartManager.DownloadFactor
            (new DownloadCartFactorRequest()
            {
                Id = id
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
        
        _downloadProcess = false;
    }
}
