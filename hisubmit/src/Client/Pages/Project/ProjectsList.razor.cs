using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;
using HiSubmit.Client.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Project;

public partial class ProjectsList
{
    [Inject] private IProjectManager ProjectManager { get; set; }

    private IEnumerable<GetAllProjectResponse> _pagedData;
    private MudTable<GetAllProjectResponse> _table;
    private GetAllProjectRequest _advancedSearch  = new GetAllProjectRequest();
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
        _loaded = true;

        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user == null) return;
        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUserId = user.GetUserId();
        }

        await base.OnInitializedAsync();
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-block" : "d-none";
    }
    private async Task<TableData<GetAllProjectResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _advancedSearch.OrderByAscending = state.SortDirection == SortDirection.Ascending;
        if (!string.IsNullOrWhiteSpace(state.SortLabel))
            _advancedSearch.Orderby = [state.SortLabel];
        
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }
        // var request = new GetAllProjectRequest();
        // if (IsAdvancedSearch)
        // {
        //     request = _advancedSearch;
        // }
        // else
        // {
        //     request.SearchString = _searchString;
        // }
        await LoadData(state.Page, state.PageSize, state, _advancedSearch);
        return new TableData<GetAllProjectResponse> { TotalItems = _totalItems, Items = _pagedData };
    }
    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllProjectRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.UserId = CurrentUserId;
        request.GetCurrentUserProjects = true;
        var response = await ProjectManager.GetAllAsync(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(request.SearchString))
                    return true;
                if (element.Title != null && element.Title.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;              
                return false;
            });
            switch (state.SortLabel)
            {
                case "ProjectNameField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Title);
                    break;

            }
            data = loadedData.ToList();
            _pagedData = data;
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

    public void AddProject()
    {
        _navigationManager.NavigateTo($"/user/project");
    }

    public void EditProject(int id)
    {
        _navigationManager.NavigateTo($"/user/project/{id}");
    }
}