using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using HiSubmit.Client.Infrastructure.Managers.FestivalChat;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Web.Components.Shared.Components.Base;
using Hisubmit.Client.SharedModels.Enums.Chats;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Festival.Reviews;

public partial class Review:BaseFestival
{
    #region Injection

    [Inject] public IFestivalManager FestivalManager { get; set; }
    [Inject] public IFestivalChatManager FestivalChatManager { get; set; }

    #endregion
    
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
        await base.CheckPermission(Permissions.Reviews.View);
        await base.OnInitializedAsync();
    }
    

    //public async Task LoadSubmits()
    //{
    //    var response = await ProjectJudgingManager.GetAll(new GetAllSubmitsRequest()
    //    {
    //        FestivalId = FestivalId
    //    });
    //    if (response.Succeeded)
    //    {
    //        _pagedDate = response.Data;
    //    }
    //    foreach (var message in response.Messages)
    //    {
    //        _snackBar.Add(message, MudBlazor.Severity.Error);
    //    }
    //}


    private void ToggleSearchFor()
    {
        _openSearchForm = !_openSearchForm;
        _displaySearchFrom = _openSearchForm ? "d-flex" : "d-none";
    }

    private async Task<TableData<GetAllReviewResponse>> ServerReload(TableState state ,System.Threading.CancellationToken  token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllReviewQuery
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
        await LoadSelectedFestivalId();
        query.PageSize = pageSize;
        query.PageNumber = pageNumber + 1;
        query.FestivalId = SelectedFestivalId;

        var response = await FestivalManager.GetAllReview(query);
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

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task SendMessage(string userId)
    {
        var response = await FestivalChatManager.GetRoomId(new TryGetRoomIdCommand()
        {
            ChatUser1 = userId,
            FestivalId = SelectedFestivalId,
            Type = ChatRoomType.FestivalUser
        }, SelectedFestivalId);

        if (response.Succeeded)
        {
            var roomId = response.Data;
            _navigationManager.NavigateTo($"/festival/Chat/{roomId}");
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);

    }
}
