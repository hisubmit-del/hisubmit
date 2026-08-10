using System;
using MudBlazor;
using AutoMapper;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Shared.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components.Forms;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetEventCateoryById;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;

namespace HiSubmit.Client.Pages.Festival.FestivalEditComponent;

public partial class FestivalEventCategory
{
    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback PrevPanel { get; set; }
    [Parameter]public bool IsAdmin { get; set; }

    #endregion

    #region Inject

    [Inject] public IEventCategoryManager EventCategoryManager { get; set; }
    [Inject] public IFestivalManager FestivalManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    #endregion


    #region Private Feild

    private bool _loaded;
    public bool _showForm;
    private bool _readOnlyMood;
    private bool _editFormClass ;
    private AddEditEventCategory AddEditCategoryForm { get; set; }
    private AddEditEventCategoryCommand Category { get; set; } = new();
    private List<GetAllEventCategoryResponse> Categories { get; set; } = new();

    #endregion


    protected override async Task OnInitializedAsync()
    {
        await GetFestivalDetail();
        await GetCategories();
        Category.FestivalId = FestivalId;
        Category.CategoryonFees = new List<UpdateDeadlineCategoryonFee>();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    private async Task GetFestivalDetail()
    {
        var response = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            _readOnlyMood = response.Data.FestivalStatus == FestivalStatus.UnderInvestigation;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task GetCategories()
    {
        var response = await EventCategoryManager.GetAllAsync(new GetAllEventCategoryQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
            Categories = response.Data;
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private bool _editViewProcessing;
    private async Task InvokeModal(int id = 0)
    {
        _editViewProcessing = true;
        var category = new AddEditEventCategoryCommand();
        if (id == 0)
        {
            var deadLineWithApplyAllCategory =
                await FestivalManager.GetAllDeadlineEntry(new GetAllDeadlineQuery()
            {
                FestivalId = FestivalId,
                ApplyToAllCategory = true
            });
            if (deadLineWithApplyAllCategory.Succeeded)
            {
                var deadLineCat = deadLineWithApplyAllCategory.Data
                    .Select(deadLine => new UpdateDeadlineCategoryonFee()
                    {
                        DeadLineId = deadLine.Id,
                        DeadLineName = deadLine.Name,
                        StandardFee = 0
                    }).ToList();

                category = new AddEditEventCategoryCommand()
                {
                    CategoryonFees = deadLineCat,
                    FestivalId = FestivalId
                };
            }
            else
                foreach (var message in deadLineWithApplyAllCategory.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
        else
        {
            var categoryDetail = await EventCategoryManager.GetById(new GetEventCategoryByIdQuery()
            {
                Id = id
            });
            if (categoryDetail.Succeeded)
            {
                category = Mapper.Map<AddEditEventCategoryCommand>(categoryDetail.Data);
            }
            else
                foreach (var message in categoryDetail.Messages)
                    _snackBar.Add(message, Severity.Error);
        }

        Category = category;
        _editFormClass = true;
        _editViewProcessing = false;
        StateHasChanged();
    }

    public bool ModifiedForm()
    {
        if (IsAdmin) return false;
        return (_editFormClass  || (AddEditCategoryForm != null && AddEditCategoryForm.ModifiedForm()));
    }

    private async Task DeleteAsync(int id, string name)
    {
        var parameters = new DialogParameters
        {
            { nameof(Shared.Dialogs.DeleteConfirmation.ContentText), string.Format(name, id) }
        };
        var options = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
           
        };
        var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await EventCategoryManager.DeleteCategory(new DeleteEventCategoryCommand
            {
                Id = id
            });
            if (response.Succeeded)
            {
                await ResetTableAndForm();
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await ResetTableAndForm();
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
            }
        }
    }


    private async Task ResetTable()
    {
        Categories = new List<GetAllEventCategoryResponse>();
        await GetCategories();
        StateHasChanged();
    }

    private async Task ResetForm(bool stateChanged = true)
    {
        _editFormClass = false;

        Category = new AddEditEventCategoryCommand
        {
            FestivalId = FestivalId,
            CategoryonFees = new List<UpdateDeadlineCategoryonFee>()
        };
        if (stateChanged)
        {
            StateHasChanged();
        }
    }

    private async Task ResetTableAndForm()
    {
        await ResetTable();
        await ResetForm(false);
        StateHasChanged();
    }

    private void HideAndResetForm()
    {
        _editFormClass =false;
    }

    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }
    private async Task GoPrev()
    {
        await PrevPanel.InvokeAsync();
    }
}