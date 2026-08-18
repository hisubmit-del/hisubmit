using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Referee;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetAll;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.User.Referee;

public partial class Refrees
{
    [Inject] public IRefereeManager RefereeManager { get; set; }
    public int FestivalId { get; set; }

    public List<GetAllProjectJudgingResponse> _pagedDate;
    private MudTable<GetAllProjectJudgingResponse> _table;
    private int _totalItems;
    private string _searchString = "";
    private bool _openSearchForm = false;
    private bool _isAdvanceSearch = false;
    private string _displaySearchFrom = "d-none";
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _loaded = true;
        await SeenNotification();
    }

    private async Task SeenNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.User,
            NotificationTypes = NotificationType.RefereeAddToProject,
            UserId = (await AuthenticationManager.CurrentUser())
                .Claims.FirstOrDefault(p=>p.Type==ClaimTypes.NameIdentifier)!.Value
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

        ////var request = new GetAllSubmitsRequest();
        ////if (IsAdvancedSearch)
        ////{
        ////    request = _advancedSearch;
        ////}
        ////else
        ////{
        ////    request.SearchString = _searchString;
        ////}
        await LoadData(state.Page, state.PageSize, state, new GetAllProjectJudgingQuery());
        return new TableData<GetAllProjectJudgingResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        _isAdvanceSearch = true;
        await _table.ReloadServerData();
        _isAdvanceSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize,
        TableState state, GetAllProjectJudgingQuery query)
    {
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.GetCurrentUser = true;

        var response = await RefereeManager.GetAllAsync(query);

        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(query.SearchString))
                    return true;
                if (element.ProjectName != null && element.ProjectName
                        .Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.FestivalName != null && element.FestivalName
                        .Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "SubmitProjectTitle":
                    loadedData = loadedData.OrderByDirection
                        (state.SortDirection, d => d.ProjectName);
                    break;
                case "SubmitDateFrom":
                    loadedData = loadedData.OrderByDirection
                        (state.SortDirection, d => d.CreatedOn);
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

    private void GoToProject(string url)
    {
        _navigationManager.NavigateTo($"/project/{url}");
    }
}
