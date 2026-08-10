using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.EventCategoris;
using HiSubmit.Client.Infrastructure.Managers.Locations;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace ClientComponents.Pages.Festival.FestivalEditComponent;

public partial class AddEditEventCategory
{
    [Parameter] public AddEditEventCategoryCommand Category { get; set; } = new();
    public List<GetAllCountryResponse> Countries { get; set; } = new();
    [Inject] private ILocationManager LocationManager { get; set; }

    [Inject] public IEventCategoryManager EventCategoryManager { get; set; }
    [Inject] private IFestivalManager FestivalManager { get; set; }

    //Event
    [Parameter] public EventCallback<bool> IsSuccessDone { get; set; }
    [Parameter] public EventCallback CancelProccess { get; set; }
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public bool IsAdmin { get; set; }

    public EditContext _EditForm { get; set; }

    private List<GetAllDeadLineResponse> DeadLines { get; set; }

    private string unit => (Category.ProjectType == ProjectType.Script_ScreenWriting
        ? Localize["Page"]
        : Localize["Minutes"]);

    private bool _processing;

    private FluentValidationValidator _fluentValidationValidator;
    public bool Loaded { get; set; }
    private bool Validated = true;
    public int CountryId { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if (Category.CategoryonFees == null)
        {
            Category.CategoryonFees = new List<UpdateDeadlineCategoryonFee>();
        }

        _EditForm = new EditContext(Category);
        await GetCountriesAsync();
        // Validated = await _fluentValidationValidator
        //     .ValidateAsync(options => 
        //         { options.IncludeAllRuleSets(); });
        await base.OnInitializedAsync();

        Loaded = true;
    }


    public async Task GetCountriesAsync()
    {
        var result = await LocationManager.GetAllCountryAsync(new GetAllCountryQuery());
        if (result.Succeeded)
        {
            Countries = result.Data;
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task<IEnumerable<int>> SearchCountries(string value, System.Threading.CancellationToken ct)
    {
        if (string.IsNullOrEmpty(value))
            return Countries.Select(x => x.Id);

        return Countries.Where(x => x.Name.Contains(value, StringComparison.InvariantCultureIgnoreCase))
            .Select(x => x.Id);
    }

    public async Task SaveAsync()
    {
        if (!Validated)
            return;

        _processing = true;
        var response = await EventCategoryManager.UpdateCategory(Category);
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            _EditForm.MarkAsUnmodified();
            await IsSuccessDone.InvokeAsync(true);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _processing = false;
    }

    public async Task Cancel()
    {
        await CancelProccess.InvokeAsync();
    }

    public bool ModifiedForm()
    {
        return _EditForm.IsModified();
    }

    private Task SelectCountry(int i)
    {
        if (Category.CountriesId.All(p => p != i))
            Category.CountriesId.Add(i);
        CountryId = 0;
        return Task.CompletedTask;
    }

    private Task DeleteCountry(int item)
    {
        Category.CountriesId.Remove(item);
        return Task.CompletedTask;
    }

    private async Task AddNewDeadline()
    {
        var title = "";
        AddEditDeadLineEntryRequest deadLine = null;

        title = Localize["Add DeadLine"];

        deadLine = new AddEditDeadLineEntryRequest()
        {
            FestivalId = FestivalId,
            ApplyToAllCategory = true,
            AddWithoutCategory = true,
            CategoryId = new List<int>()
        };

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
            var newDeadLine = (GetDeadLineByIdResponse)result.Data;
            Category.CategoryonFees.Add(new UpdateDeadlineCategoryonFee()
            {
                DeadLineId = newDeadLine.Id,
                DeadLineName = newDeadLine.Name,
                StandardFee = 0
            });
        }
    }
}