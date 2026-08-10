using System.Collections.Generic;
using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Notifications;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Notifications.Queries;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Festival.Notifications;

public partial class Notifications
{
     #region Injection

    [Inject] public INotificationManager NewManager { get; set; }

    #endregion

    private List<GetAllNotificationResponse> _pagedDate { get; set; }
    public GetAllNotificationQuery Query { get; set; }


    private MudTable<GetAllNotificationResponse> _table;
    private GetAllNotificationQuery _advancedSearch { get; set; } = new GetAllNotificationQuery();
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
     //   await base.CheckPermission(Permissions.FestivalNews.View);
        await base.OnInitializedAsync();
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllNotificationResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllNotificationQuery
        {
            Seen = null,
        };

        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllNotificationResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllNotificationQuery query)
    {
        await LoadSelectedFestivalId();
        
        query.FestivalId=SelectedFestivalId;
        query.SiteAccountType = SiteAccountType.Festival;
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;

        var response = await NewManager.GetFestivalNotifications(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
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
}
