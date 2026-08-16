using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HiSubmit.Client.Pages.Project;

public partial class AddEditProjectCredit
{
    #region Injection
    [Inject]
    public IProjectManager ProjectManager { get; set; }
    [Inject]
    public IMapper Mapper { get; set; }
    #endregion

    #region Parameters
    [CascadingParameter]
    public int ProjectId { get; set; }
    [Parameter]
    public EventCallback NextPanel { get; set; }

    #endregion

    #region Private Filled
    private UpdateProjectCreditsRequest _model { get; set; } = new();
    private EditContext _editContext { get; set; }
    private FluentValidationValidator _fluentValidationValidator { get; set; }
    private bool _loaded { get; set; }
    private bool _processing { get; set; }
    private bool Validated { get; set; } = true;
    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await LoadCredits();
        _editContext = new EditContext(_model);
        _loaded = true;
    }
    #endregion


    private async Task LoadCredits()
    {
        var response = await ProjectManager.GetAllProjectCreditAsync(new GetAllProjectCreditQuery()
        {
            ProjectId = ProjectId,
            WithInclude = true
        });

        if (response.Succeeded)
        {
            if (response.Data.Any())
            {
                _model.Credits = Mapper.Map<List<AddEditProjectCreditCommand>>(response.Data);
            }
            else
            {
                _model.Credits = new List<AddEditProjectCreditCommand>()
                {
                    new()
                        {ProjectItemPeople = [new AddEditProjectCreditItemCommand()] }
                };
            }
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    private async Task AddCredit()
    {
        await Task.Run(() =>
        {

            _model.Credits.Add(new AddEditProjectCreditCommand()
            {
                ProjectId = ProjectId,
                ProjectItemPeople = new List<AddEditProjectCreditItemCommand>()
            });
        });
    }
    private async Task AddPerson(AddEditProjectCreditCommand credit)
    {
        await Task.Run(() =>
        {
            credit.ProjectItemPeople.Add(new AddEditProjectCreditItemCommand());
        });
    }
    private async Task DeleteCredit(AddEditProjectCreditCommand credit)
    {
        await Task.Run(() =>
        {
            _model.Credits.Remove(credit);
        });
    }
    private async Task DeletePerson(AddEditProjectCreditItemCommand person, AddEditProjectCreditCommand credit)
    {
        await Task.Run(() =>
        {
            credit.ProjectItemPeople.Remove(person);
        });
    }
    public async Task<bool> SaveAsync()
    {
        _processing = true;
        Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        if (Validated)
        {
            _model.ProjectId = ProjectId;
            var result = await ProjectManager.UpdateCredit(_model);
            _processing = false;
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                await LoadCredits();
                _editContext = new EditContext(_model);
                //  ProjectId = result.Data;
                _editContext.MarkAsUnmodified();
                    
                return true;
            }

            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        _processing = false;
        return false;
    }
    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }
    public async Task<bool> CheckForm()
    {
        if (_editContext.IsModified())
        {
            var parameters = new DialogParameters();
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
            var dialog = _dialogService.Show<Shared.Dialogs.SaveAndNext>(localizer["Warning"], parameters, options);
            var result = await dialog.Result;
            if (!result.Canceled)
            {
                if (!await SaveAsync())
                {
                    return false;
                }
            }
            return true;
        }
        return true;
    }

    public bool ModifiedForm()
    {
        return _editContext.IsModified();
    }
}