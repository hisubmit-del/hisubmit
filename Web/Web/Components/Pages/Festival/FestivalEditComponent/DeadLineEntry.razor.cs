using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Web.Components.Pages.Festival.FestivalEditComponent;

public partial class DeadLineEntry
{
    #region Injection

    [Inject] public IFestivalManager FestivalManager { get; set; }
    [Inject] public IMapper Mapper { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public bool ReadOnlyMood { get; set; }
    [Parameter]public bool IsAdmin { get; set; }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await GetDeadLines();
        await base.OnInitializedAsync();
    }

    #endregion

    #region Private Filled

    private List<GetAllDeadLineResponse> DeadLines { get; set; } = new();

    #endregion


    private async Task GetDeadLines()
    {
        var result = await FestivalManager.GetAllDeadlineEntry(new GetAllDeadlineQuery
        {
            FestivalId = FestivalId
        });
        if (result.Succeeded)
        {
            DeadLines = result.Data;
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task InvokeModal(int id = 0)
    {
        var title = "";
        AddEditDeadLineEntryRequest deadLine = null;

        if (id == 0)
        {
            title = Localize["Add DeadLine"];
            deadLine = new AddEditDeadLineEntryRequest()
            {
                FestivalId = FestivalId,
                ApplyToAllCategory = true,
                CategoryId = new List<int>()
            };
        }
        else
        {
            title = Localize["Updated DeadLine"];
            var deadLineDetail = await FestivalManager.GetDeadlineEntryDetail(new GetDeadLineByIdQuery
            {
                Id = id,
                FestivalId = FestivalId
            });
            deadLine = Mapper.Map<AddEditDeadLineEntryRequest>(deadLineDetail.Data);
        }

        var parameter = new DialogParameters
        {
            { nameof(AddEditDeadLineEntry.DeadLine), deadLine },
            { nameof(AddEditDeadLineEntry.FestivalId), FestivalId }
        };
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            
        };
        var dialog = _dialogService.Show<AddEditDeadLineEntry>(title, parameter, options);

        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await Reset();
        }
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
        var dialog = _dialogService
            .Show<Shared.Dialogs.DeleteConfirmation>(Localize["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            var response = await FestivalManager.DeleteDeadLineEntry(new DeleteDeadLineEntryCommand
            {
                Id = id,
                FestivalId = FestivalId
            });
            if (response.Succeeded)
            {
                await Reset();
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                await Reset();
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }
    }

    private async Task Reset()
    {
        DeadLines = new List<GetAllDeadLineResponse>();
        await GetDeadLines();
    }
}