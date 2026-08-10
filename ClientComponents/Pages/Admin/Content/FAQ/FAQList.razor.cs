using System;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using HiSubmit.Client.Infrastructure.Managers.StaticPages;
using ClientComponents.Shared.Dialogs;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientComponents.Pages.Admin.Content.FAQ;

public partial class FAQList
{
    #region Inject
    [Inject] public IStaticPageManager StaticPageManager { get; set; }

    #endregion

    #region Parameters
    [Parameter] public int FestivalIdParam { get; set; }

    #endregion
    public int FestivalId { get; set; }



    public List<GetAllStaticPageResponse> _pagedDate { get; set; }
    public GetAllStaticPageRequest Request { get; set; }


    private MudTable<GetAllStaticPageResponse> _table;
    private GetAllStaticPageRequest _advancedSearch { get; set; } = new GetAllStaticPageRequest();
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

    private async Task<TableData<GetAllStaticPageResponse>>
        ServerReload(TableState state, System.Threading.CancellationToken token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new GetAllStaticPageRequest
        {
            Type = ContentType.Faq
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
        return new TableData<GetAllStaticPageResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task AdvancedSearch()
    {
        IsAdvancedSearch = true;
        await _table.ReloadServerData();
        IsAdvancedSearch = false;
    }


    private async Task LoadData(int pageNumber, int pageSize, TableState state, GetAllStaticPageRequest request)
    {
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        //request.FestivalId = FestivalId;
        var response = await StaticPageManager.GetAllAsync(request);
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


    private async Task Delete(int id, string title)
    {
        var parameters = new DialogParameters { { nameof(DeleteConfirmation.ContentText), title } };

        var options = new DialogOptions
        { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, };
        var dialog = _dialogService.Show<DeleteConfirmation>(Localize["Delete Confiramtion"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await StaticPageManager.DeleteAsync(new DeleteStaticPageCommand()
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

    private async Task InvokeModal(int id)
    {
        var parameters = new DialogParameters();
        var model = new AddEditStaticPageRequest()
        {
            Type = ContentType.Faq,

            IsEnable = true
        };

       if(id!=0)
        {
            var res = await StaticPageManager.GetDetailAsync(new GetDetailStaticPageQuery()
            {
                Id = id
            });


            if (res.Succeeded)
            {
                var item = res.Data;

                model.Id = item.Id;
                model.Link = item.Link;
                model.Title = item.Title;
                model.Content = item.Content;
                model.IsEnable = true;
            }
        }

        parameters.Add(nameof(AddEditStaticPageModal.Model), model);

        var options = new DialogOptions
        { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, };
        var dialog = _dialogService.Show<AddEditStaticPageModal>(model.Id == 0 ? Localize["Create"] : Localize["Edit"],
            parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }
}
