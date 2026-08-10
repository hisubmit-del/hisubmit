using System;
using MudBlazor;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using HiSubmit.Web.Components.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using HiSubmit.Web.Components.Shared.Components.Base;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using HiSubmit.Client.Infrastructure.Managers.FestivalNews;

namespace HiSubmit.Web.Components.Pages.Festival.News;

public partial class News : BaseFestival
{
    #region Injection

    [Inject] public IFestivalNewsManager NewManager { get; set; }

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
        await base.CheckPermission(Permissions.FestivalNews.View);
        await base.OnInitializedAsync();
    }

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

        var query = new GetAllNewRequest
        {
            FestivalId = SelectedFestivalId
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
        await LoadSelectedFestivalId();
        //  await LoadFestivalId();
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.FestivalId = SelectedFestivalId;
        request.GetFestivalNews = true;
        var response = await NewManager.GetAllAsync(request, SelectedFestivalId);
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
        _navigationManager.NavigateTo("/festival/new");
    }

    private void Edit(int id)
    {
        _navigationManager.NavigateTo($"/festival/new/{id}");
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
            var response = await NewManager.DeleteAsync(new DeleteNewCommand()
            {
                Id = id
            }, SelectedFestivalId);
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