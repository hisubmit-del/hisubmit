using System;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.ProjectImages;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Project.Files;

public partial class AddPhotoModal
{
    #region Parameter

    [Parameter] public int ProjectId { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion

    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }

    #endregion

    #region Private Filled

    private AddProjectImageCommand _model = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    #endregion

    #region Override

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        _model.ProjectId = ProjectId;
        return base.OnAfterRenderAsync(firstRender);
    }
    #endregion

    private async Task SaveAsync()
    {
        Console.WriteLine("Project Id:{0}",_model.ProjectId);
        _processing = true;
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await ProjectManager.AddProjectImage(_model);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
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

    private void Cancel()
    {
        MudDialog.Close();
    }
}