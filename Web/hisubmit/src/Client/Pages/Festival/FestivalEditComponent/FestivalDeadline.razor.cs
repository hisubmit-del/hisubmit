using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;

namespace HiSubmit.Client.Pages.Festival.FestivalEditComponent;

public partial class FestivalDeadline
{
    #region Parameter

    [Parameter]public bool IsAdmin { get; set; }
    [Parameter] public int FestivalId { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback PrevPanel { get; set; }

    #endregion

    #region Injection

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }

    #endregion

    #region Private Filled

    private AddEditFestivalDeadlineCommand _festival { get; set; } = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; } = true;
    private bool Loaded { get; set; }
    private bool _proccessing { get; set; }
    private EditContext _editForm { get; set; }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await GetFestivalAsync();
        _editForm = new EditContext(_festival);
        await base.OnInitializedAsync();
        Loaded = true;
    }

    #endregion


    private async Task GetFestivalAsync()
    {
        var result = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery()
        {
            FestivalId = FestivalId
        });

        if (result.Succeeded)
        {
            _festival = Mapper.Map<AddEditFestivalDeadlineCommand>(result.Data);
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
        if (_festival.FestivalStatus == FestivalStatus.UnderInvestigation) return true;
        Validated = _fluentValidationValidator
            .Validate((option) => option.IncludeAllRuleSets());
        
        if (Validated)
        {
            _proccessing = true;
            var response = await FestivalManager.SaveDeadLineAsync(_festival);
            _proccessing = false;
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _editForm.MarkAsUnmodified();
                return true;
            }

            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
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
        return _editForm.IsModified();
    }

}