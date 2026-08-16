using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Infrastructure.Managers.AdminNews;
using HiSubmit.Client.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Admin.Content.News;

public partial class News
{
    #region Injection

    [Inject] public IAdminNewManager NewManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int? FestivalId { get; set; }

    #endregion

    private List<GetAllNewResponse> _pagedDate { get; set; }
    public GetAllNewRequest Request { get; set; }


    private MudTable<GetAllNewResponse> _table;
    private GetAllNewRequest _advancedSearch { get; set; } = new GetAllNewRequest();
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
    //    await LoadFestivalId();
        await base.OnInitializedAsync();
    }

    // private async Task LoadFestivalId()
    // {
    //     if (FestivalIdParam != 0)
    //     {
    //         FestivalId = FestivalIdParam;
    //     }
    //     else
    //     {
    //         FestivalId = await _localStorage.GetItemAsync<int>(StorageConstants.Local.FestivalId);
    //     }
    // }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllNewResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllNewRequest();
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
        return new TableData<GetAllNewResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllNewRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.GetFestivalNews = true;
        request.FestivalId = FestivalId;
        var response = await NewManager.GetAllAsync(request);
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

    private void Add()
    {
        _navigationManager.NavigateTo("/admin/new");
    }

    private async Task Edit(int id)
    {
        _navigationManager.NavigateTo($"/admin/new/{id}");
    }

    private async Task Delete(int id, string title)
    {
        var parameters = new DialogParameters { { nameof(DeleteConfirmation.ContentText), title } };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<DeleteConfirmation>(Localize["Delete Confiramtion"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await NewManager.DeleteAsync(new DeleteNewCommand()
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

    private async Task ChangeChecked(int contextId, bool b)
    {
        var response = await NewManager.UpdateEnableAsync(new UpdateEnableNewCommand()
        {
            Id = contextId,
            IsEnable = b
        });
        if (response.Succeeded)
            await _table.ReloadServerData();
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }
}
