using AutoMapper;
using Blazored.FluentValidation;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Client.Pages.Festival.FestivalEditComponent;

public partial class ContactAndVenue
{
    #region Inject

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int FestivalId { get; set; }
[Parameter]public bool IsAdmin { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback PrevPanel { get; set; }

    #endregion

    #region Private Feild

    private bool _loaded;
    private bool _processing;
    private EditContext _EditForm;
    private bool _validated = true;
    private FluentValidationValidator _fluentValidationValidator;
    private AddEditFestivalContactCommand _festival { get; set; } = new();

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await GetFestivalAsync();
        _EditForm = new EditContext(_festival);
        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

    private async Task GetFestivalAsync()
    {
        var result = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId,
            WithInclude = true
        });

        if (result.Succeeded)
        {
            _festival = Mapper.Map<AddEditFestivalContactCommand>(result.Data);
            if (_festival.Address == null)
            {
                _festival.Address = new AddEditAddressCommand() { FestivalId = FestivalId };
            }

            if (_festival.SubmissionAddress == null)
            {
                _festival.SubmissionAddress = new AddEditAddressCommand() { SubmissionFestivalId = FestivalId };
            }
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    public async Task<bool> SaveAsync()
    {
        if (_festival.FestivalStatus == FestivalStatus.UnderInvestigation)
            return true;

        _validated = await _fluentValidationValidator
            .ValidateAsync(
                (option) => option.IncludeAllRuleSets());
        if (_validated)
        {
            _processing = true;
            var response = await FestivalManager.SaveContactAsync(_festival);
            _processing = false;
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _EditForm.MarkAsUnmodified();
                return true;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        return false;
    }

    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }

    private async Task GoPrev()
    {
        await PrevPanel.InvokeAsync();
    }
    public bool ModifiedForm()
    {
        return _EditForm.IsModified();
    }
}