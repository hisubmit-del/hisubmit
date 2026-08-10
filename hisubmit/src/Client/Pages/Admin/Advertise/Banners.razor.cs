using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;
using HiSubmit.Client.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Advertise;

public partial class Banners
{
    #region Injection

    [Inject] public IAdminAdvertiseManager AdvertiseManager { get; set; }

    #endregion

    private List<GetAllAdvertiseBannerResponse> _pagedDate { get; set; }
    public GetAllAdvertiseBannerRequest Request { get; set; }


    private MudTable<GetAllAdvertiseBannerResponse> _table;
    private GetAllAdvertiseBannerRequest _advancedSearch { get; set; } = new GetAllAdvertiseBannerRequest();
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
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllAdvertiseBannerResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllAdvertiseBannerRequest();
        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllAdvertiseBannerResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllAdvertiseBannerRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        //request.FestivalId = FestivalId;
        var response = await AdvertiseManager.GetAllBanner(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(request.SearchString))
                    return true;
                if (element.Title != null &&
                    element.Title.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
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

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task InvokeModal(GetAllAdvertiseBannerResponse response = null)
    {
        var command = new AddEditAdvertiseBannerRequest();
        if (response != null)
        {
            command = new AddEditAdvertiseBannerRequest()
            {
                Id = response.Id,
                Url = response.Url,
                Title = response.Title,
                Position = response.Position,
                OpenDateTime = response.OpenDateTime,
                CloseDateTime = response.CloseDateTime,
            };
        }

        var parameters = new DialogParameters
        {
            { nameof(AddAdvertiseBanner.Model), command }
        };
        var options = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
           
        };
        var dialog = _dialogService.Show<AddAdvertiseBanner>(Localize["Add Advertise"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
            OnSearch("");
    }
    


    private async Task Delete(int id, string title)
    {
        var parameters = new DialogParameters { { nameof(DeleteConfirmation.ContentText), title } };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<DeleteConfirmation>(Localize["Delete Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await AdvertiseManager.DeleteBanner(new DeleteAdvertiseBannerRequest
            {
                Id = id
            });
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await _table.ReloadServerData();
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
}