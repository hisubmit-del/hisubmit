using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Web.Components.Shared.Components;
using HiSubmit.Web.Components.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Web.Components.Pages.User.Submits;

public partial class SubmitsList
{
    #region Injection

    [Inject] public ISubmitManager SubmitManager { get; set; }
    [Inject] public IProjectManager ProjectManager { get; set; }

    #endregion

    #region Private Field

    private List<GetAllSubmitsResponse> PagedData { get; set; }
    private GetAllSubmitsRequest Request { get; set; }
    private List<GetAllProjectResponse> Projects { get; set; }

    private MudTable<GetAllSubmitsResponse> _table;
    private GetAllSubmitsRequest _advancedSearch { get; set; } = new();
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

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadProjects();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllSubmitsResponse>> ServerReload(TableState state ,System.Threading.CancellationToken token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllSubmitsRequest();
        if (IsAdvancedSearch)
        {
            query = _advancedSearch;
        }
        else
        {
            query.SearchString = _searchString;
        }

        await LoadData(state.Page, state.PageSize, state, query);
        return new TableData<GetAllSubmitsResponse> { TotalItems = _totalItems, Items = PagedData };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }

    private async Task LoadProjects()
    {
        var response = await ProjectManager.GetAllAsync(new GetAllProjectRequest()
        {
            PageSize = 1000
        });
        if (response.Succeeded)
        {
            Projects = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllSubmitsRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.GetCurrentUserSubmits = true;

        var response = await SubmitManager.GetAll(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(request.SearchString))
                    return true;
                if (element.ProjectTitle != null &&
                    element.ProjectTitle.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.FestivalName != null &&
                    element.FestivalName.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.ProjectTitle);
                    break;
                case "SubmitDateFrom":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.SubmitDate);
                    break;
            }

            data = loadedData.ToList();
            PagedData = data;
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

    private async Task WithDraw(int id)
    {
        var parameters = new DialogParameters
        {
            { nameof(WithDrawDialog.SubmitId), id },
            { nameof(WithDrawDialog.Color), Color.Warning },
            { nameof(WithDrawDialog.ButtonText), $"{Localize["WithDraw"]}" },
            { nameof(WithDrawDialog.ContentText), $"{Localize["With Draw Confirmation"]}" },
        };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

        var dialog = _dialogService.Show<WithDrawDialog>(Localize["Withdraw"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task Review(int id, CommentType type = CommentType.Review)
    {
        var parameters = new DialogParameters
        {
            { nameof(ReviewDialog.Type), type },
            { nameof(ReviewDialog.FestivalId), id }
        };
        var options = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            
        };
        var dialog = _dialogService.Show<ReviewDialog>(Localize["Review"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
}
