using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.JudgingProjects;
using Web.Components.Pages.Festival.JudgingProjects;
using Web.Components.Pages.Festival.Submits;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.RemovedUserFromProject;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;

namespace Web.Components.Pages.Festival.ProjectJudgings;

public partial class SubmitDetail
{
    #region Inject

    [Inject] private ILocalStorageService LocalStorageService { get; set; }
    [Inject] private IProjectJudgingManager ProjectJudgingManager { get; set; }
    [Inject] private IFestivalSubUserManager FestivalSubUserManager { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int SubmitId { get; set; }
    [Parameter] public string ProjectName { get; set; }

    #endregion

    #region Private Field

    private bool _dense;
    private bool _loaded;
    private int _totalItems;
    private bool _bordered;
    private int _currentPage;
    private bool _striped = true;
    private string _currentUserId;
    private bool _openSearchForm;
    private bool IsAdvancedSearch;
    private string _searchString = "";
    private GetAllProjectJudgingQuery _query;
    private string _displaySearchFrom = "d-none";
    private MudTable<GetAllProjectJudgingResponse> _table;
    private List<GetAllProjectJudgingResponse> _pagedDate;

    #endregion


    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.Submits.AddToReferee);
        await base.OnInitializedAsync();
        _loaded = true;
        await NotificationManager.SeenNotifications(new SeenNotificationCommand()
        {
            AccountType = SiteAccountType.Festival,
            NotificationTypes = NotificationType.FestivalRefereeSubmitJudgingResult,
            FestivalId = SelectedFestivalId,
        });
        NotificationService.ChangeNotificationBar();
    }


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllProjectJudgingResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllProjectJudgingQuery
        {
            //if (IsAdvancedSearch)
            //{
            //    request = _advancedSearch;
            //}
            //else
            //{
            //    request.SearchString = _searchString;
            //}
            SearchString = _searchString
        };
        await LoadData(state.Page, state.PageSize, state, query);
        return new TableData<GetAllProjectJudgingResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllProjectJudgingQuery query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.SubmitId = SubmitId;
        var response = await ProjectJudgingManager.GetAll(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.ProjectName != null &&
                    element.ProjectName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.JudgingButtonName != null &&
                    element.JudgingButtonName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.ProjectName);
                    break;
                case "SubmitDateFrom":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.JudgingButtonName);
                    break;
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

    private async Task OnSearch(string text)
    {
        _searchString = text?.Trim() ?? string.Empty;
        if (_table is not null)
            await _table.ReloadServerData();
    }

    private async Task SubmitFinalResult()
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(FinalResultModal.SubmitId), new List<int> { SubmitId });

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<FinalResultModal>(Localize["Submit final result"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private Task GoToDetail(int id)
    {
        _navigationManager.NavigateTo($"/JudgingDetail/{id}");
        return Task.CompletedTask;
    }

    private async Task AddToReferee()
    {
        var submitId = SubmitId;
        var parameters = new DialogParameters
        {
            { nameof(AddReferesToProject.FestivalId), SelectedFestivalId },
            { nameof(AddReferesToProject.SubmitId),new List<int>{submitId} }
        };
        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog =
            _dialogService.Show<AddReferesToProject>(Localize["Add Judge To Projects"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task RemoveFromProject(int contextId)
    {
        var result = await FestivalSubUserManager
            .RemovedUserFromProject(new RemovedUserFromProjectCommand
        {
            Id = contextId
        }, SelectedFestivalId);
        _snackBar.Add(result.Messages[0], result.Succeeded ? Severity.Success : Severity.Error);
    }
}
