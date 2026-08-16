using AdminDashboard.Wasm.Models;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllEventCategory;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Web.Components.Shared.Components;
using Hisubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Festival.FestivalEditComponent;

public partial class AddEditDeadLineEntry
{
    [Inject]
    public IFestivalManager FestivalManager { get; set; }

    [Inject]
    public IEventCategoryManager EventCategoryManager { get; set; }

    [Parameter]
    public AddEditDeadLineEntryRequest DeadLine { get; set; } = new();
    [Parameter]
    public int FestivalId { get; set; }

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; }= true;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }

    public bool Loaded { get; set; }

    private bool _processing;

    public List<GetAllEventCategoryResponse> Categories { get; set; } = new();

    private List<CheckBoxItem<int>> _CategoriesItems { get; set; } = new();

    public GroupCheckBox CategoryList { get; set; }
    private async Task GetCategories()
    {
        var response=await EventCategoryManager.GetAllAsync(new GetAllEventCategoryQuery
        {
            FestivalId = FestivalId
        });
        if (response.Succeeded)
        {
            Categories = response.Data;
            
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
            
    }

    protected override async Task OnInitializedAsync()
    {
        await GetCategories();
        generateCheckBoxItems();
        await base.OnInitializedAsync();
        Loaded = true;
    }
    private void generateCheckBoxItems()
    {

        foreach (var cats in Categories)
        {
            var catItem = new CheckBoxItem<int>()
            {
                Value = cats.Id,
                Name = cats.Name,
                IsSelected = DeadLine.CategoryId.Any(id => id == cats.Id)
            };
            _CategoriesItems.Add(catItem);
        }
    }

    public async Task SaveAsync()
    {
        _processing = true;
        if (!DeadLine.ApplyToAllCategory)
        {
            DeadLine.CategoryId = CategoryList.SelectedItems;
        }

        Validated = _fluentValidationValidator.Validate((option) => option.IncludeAllRuleSets());
        if (Validated)
        {              
            var response = await FestivalManager.AddEditDeadLineEntry(DeadLine);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);

                MudDialog.Close(response.Data);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        _processing = false;
    }

    public void Cancel()
    {
        MudDialog.Cancel();
    }
}