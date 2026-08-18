using System;
using MudBlazor;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Enums.Chats;
using Microsoft.AspNetCore.Components;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Features.Notifications.Commands;
using HiSubmit.Client.Infrastructure.Managers.FestivalChat;
using HiSubmit.Client.Infrastructure.Managers.AdminReview;

namespace Web.Components.Pages.Admin.Festivals;

public partial class ViolationReport
{
    #region Injection

    [Inject] public IFestivalManager FestivalManager { get; set; }
    [Inject] private IAdminReviewManager AdminReviewManager { get; set; }
    [Inject] public IFestivalChatManager FestivalChatManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int? FestivalIdParam { get; set; }

    #endregion

  private int? FestivalId { get; set; }

    private List<GetAllReviewResponse> _pagedDate { get; set; }
    public GetAllReviewQuery Query { get; set; }

    private MudTable<GetAllReviewResponse> _table;
    private GetAllReviewQuery _advancedSearch { get; set; } = new();
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
        await LoadFestivalId();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await SeenNotification();
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadFestivalId()
    {
        if (FestivalIdParam != null)
            FestivalId = FestivalIdParam;      
    }

    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllReviewResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
            state.Page = 0;

        var query = new GetAllReviewQuery
        {
            FestivalId = FestivalId
        };

        if (IsAdvancedSearch)
            query = _advancedSearch;
        else
            query.SearchString = _searchString;

        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllReviewResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllReviewQuery query)
    {
        await LoadFestivalId();
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = FestivalId;

        var response = await AdminReviewManager.GetAll(query);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (element.UserFullName != null &&
                    element.UserFullName.Contains(query.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                // if (element.Name != null &&
                //     element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            });
            switch (state.SortLabel)
            {
                case "Title":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.UserFullName);
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

    private async Task OnSearch(string text)
    {
        _searchString = text?.Trim() ?? string.Empty;
        if (_table is not null)
            await _table.ReloadServerData();
    }

    private async Task SendMessage(string userId)
    {
        var roomId = await FestivalChatManager.GetRoomId(new TryGetRoomIdCommand()
        {
            ChatUser1 = userId,
            ChatWithAdmin = true,
            Type = ChatRoomType.AdminUser
        }, FestivalId.Value);
        _navigationManager.NavigateTo($"/admin/newChat/{roomId}");
    }

    private async Task SeenNotification()
    {
        await NotificationManager.SeenNotifications(new SeenNotificationCommand
        {
            AccountType = SiteAccountType.Admin,
            NotificationTypes = NotificationType.AdminReportViolationFestival,
        });
        NotificationService.ChangeNotificationBar();
    }
}
