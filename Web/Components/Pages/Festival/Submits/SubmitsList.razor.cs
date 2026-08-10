using System;
using MudBlazor;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Web.Components.Pages.Festival.JudgingProjects;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalPeriods;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace Web.Components.Pages.Festival.Submits;

public partial class SubmitsList
{
    #region Injection

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] public ISubmitManager SubmitManager { get; set; }

    #endregion

    private GetAllFestivalPeriodsResponse _festivalperiods = new();
    private List<GetAllSubmitsResponse> PagedDate { get; set; }
    public GetAllSubmitsRequest Request { get; set; }

    private MudTable<GetAllSubmitsResponse> _table;
    private GetAllSubmitsRequest _advancedSearch = new();
    private string CurrentUserId { get; set; }
    public HashSet<GetAllSubmitsResponse> _selectedItems = new();

    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool IsAdvancedSearch = false;
    private string _displaySearchFrom = "d-none";
    private bool _loaded;
    private bool _loadedData;
    private int _selectedFestivalId;

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await base.CheckPermission(Permissions.Submits.View);
        _selectedFestivalId = SelectedFestivalId;
        await base.OnInitializedAsync();
        _loaded = true;
        await LoadAllPeriods();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await SeenNotification();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion


    private async Task SeenNotification()
    {
        await LoadSelectedFestivalId();
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            FestivalId = SelectedFestivalId,
            AccountType = SiteAccountType.Festival,
            NotificationTypes = NotificationType.FestivalNewSubmit
        });
        NotificationService.ChangeNotificationBar();
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllSubmitsResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }
        
        _advancedSearch.OrderByAscending = state.SortDirection == SortDirection.Ascending;
        if (!string.IsNullOrWhiteSpace(state.SortLabel))
            _advancedSearch.Orderby = [state.SortLabel];

        await LoadData(state.Page, state.PageSize, state, _advancedSearch);
        return new TableData<GetAllSubmitsResponse> { TotalItems = _totalItems, Items = PagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllSubmitsRequest request)
    {
        _loadedData = false;
        await LoadSelectedFestivalId();
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.FestivalId = SelectedFestivalId;
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
            PagedDate = data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _loadedData = true;
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }


    private async Task AddToReferee(int submitId)
    {
        var parameters = new DialogParameters
        {
            { nameof(AddReferesToProject.FestivalId), SelectedFestivalId },
            { nameof(AddReferesToProject.SubmitId), new List<int>(){submitId} }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
           
        };
        var dialog =
            _dialogService.Show<AddReferesToProject>(Localize["Add Judge To Projects"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task JudgingResult(int submitId, string projectName)
    {
        _navigationManager.NavigateTo($"/festival/judgingResult/{submitId}/{projectName}");
    }

    private async Task SubmitFinalResult(int submitId)
    {
        await ShowNotifyModal([submitId]);
    }

    private async Task LoadAllPeriods()
    {
        var response = await FestivalManager.GetAllFestivalPeriods(new GetAllFestivalPeriodsQuery()
        {
            FestivalId = SelectedFestivalId
        });

        if (response.Succeeded)
        {
            _festivalperiods = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task AddMultiProjectToReferee()
    {
        var parameters = new DialogParameters
        {
            { nameof(AddReferesToProject.FestivalId), SelectedFestivalId },
            { nameof(AddReferesToProject.SubmitId), _selectedItems.Select(p=>p.Id).ToList() }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
           
        };
        var dialog =
            _dialogService.Show<AddReferesToProject>(Localize["Add Judge To Projects"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task NotifySelectedItems()
    {
        await ShowNotifyModal(_selectedItems.Select(p=>p.Id).ToList());
    }

    private async Task ShowNotifyModal(List<int> submitsId)
    {
        var parameters = new DialogParameters
        {
            { nameof(FinalResultModal.SubmitId),submitsId }
        };

        var options = new DialogOptions
            { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
        var dialog = _dialogService.Show<FinalResultModal>(Localize["Submit final result"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    
}