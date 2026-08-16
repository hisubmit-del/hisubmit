using System;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using HiSubmit.Client.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Client.Infrastructure.Managers.Payments;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Client.Pages.Festival.Payments.DiscountCodes;

public partial class DiscountCodesList
{
    #region Injection

    [Inject] public IDiscountCodeManager DiscountCodeManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    #endregion

    private List<GetAllDiscountCodeResponse> _pagedDate { get; set; }
    private DiscountCodeFilter _filter = new();

    private MudTable<GetAllDiscountCodeResponse> _table;
    private int _totalItems;
    private string _searchString = "";

    private bool _loaded;
    protected override async Task OnInitializedAsync()

    {
        await base.CheckPermission(Permissions.FestivalNews.View);
        await base.OnInitializedAsync();
    }

    private async Task<TableData<GetAllDiscountCodeResponse>> ServerReload(TableState state, System.Threading.CancellationToken token)
    {
        _loaded = false;
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }

        var query = new DiscountCodeFilter
        {
            FestivalId = SelectedFestivalId
        };


        await LoadData(state.Page, state.PageSize, state, query);
        _loaded = true;
        return new TableData<GetAllDiscountCodeResponse> { TotalItems = _totalItems, Items = _pagedDate };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state, DiscountCodeFilter request)
    {
        await LoadSelectedFestivalId();
        //  await LoadFestivalId();
        request.PageSize = pageSize;
        request.PageNumber = pageNumber + 1;
        request.FestivalId = SelectedFestivalId;
        //request.GetFestivalNews = true;
        var response = await DiscountCodeManager.GetAllDiscountCode(request);
        if (response.Succeeded)
        {
            _totalItems = response.TotalCount;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(request.SearchString))
                    return true;
                if (element.Code != null &&
                    element.Code.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                // if (element.Name != null &&
                //     element.Name.Contains(request.SearchString, StringComparison.OrdinalIgnoreCase))
                return true;
            });
            switch (state.SortLabel)
            {
                case "Title":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Code);
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

    private async Task AddEdit(GetAllDiscountCodeResponse discountCode=null)
    {
        var parameters = new DialogParameters<AddEditDiscountCodeModal>
        { 
        { nameof(AddEditDiscountCodeModal.Model), discountCode != null
            ? Mapper.Map<AddEditDiscountCodeRequest>(discountCode)
            : new AddEditDiscountCodeRequest() { FestivalId = SelectedFestivalId } }
        };


        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
        };
        var dialog = _dialogService.Show<AddEditDiscountCodeModal>(Localize["Add codes"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _table.ReloadServerData();
        }
    }

    private async Task Delete(int id, string title)
    {
        var parameters = new DialogParameters { { nameof(DeleteConfirmation.ContentText), title } };

        var options = new DialogOptions
        { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, };
        var dialog = _dialogService.Show<DeleteConfirmation>(Localize["Delete Confirmation"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await DiscountCodeManager.Delete(new BaseDeleteRequest()
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

    private async Task ChangeStatus(GetAllDiscountCodeResponse context, bool status)
    {
        _table.Loading = true;
        var req = new ChangeDiscountCodeStatusRequest()
        {
            FestivalId = context.FestivalId,
            Enable =status,
            Id = context.Id
        };
        var response = await DiscountCodeManager.ChangeStatus(req);
        _snackBar.Add(response.Messages[0], response.Succeeded ? Severity.Success : Severity.Error);
        context.Enable=status;
        _table.Loading = true;
    }
}
