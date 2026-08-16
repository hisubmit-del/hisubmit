using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace HiSubmit.Client.Pages.Project.Files;

public partial class ExternalFile
{
    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int ProjectId { get; set; }
    [Parameter] public EventCallback Canceled { get; set; }
    [Parameter] public EventCallback FileUploaded { get; set; }
    [Parameter] public EventCallback UploadCompleted { get; set; }
    #endregion

    #region Private Filled

    private bool _validated;
    private bool _loaded;
    private bool _processing;
    private EditContext _editContext;
    private AddEditProjectFileURLRequest _file = new();
    private FluentValidationValidator _fluentValidationValidator;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        _editContext = new EditContext(_file);
        await base.OnInitializedAsync();
    }

    #endregion


    private async Task Cancel()
    {
        await Canceled.InvokeAsync();
    }

    public async Task<bool> SaveAsync()
    {
        _processing = true;
        _validated = _fluentValidationValidator.Validate((p) => p.IncludeAllRuleSets());
        if (_validated)
        {
            return await UpdateFileUrl(_file);
        }
        await FileUploaded.InvokeAsync();
        _processing = false;
        return false;
    }

    private async Task<bool> UpdateFileUrl(AddEditProjectFileURLRequest request)
    {
        var result = false;
        request.ProjectId = ProjectId;
        var response = await ProjectManager.UpdateProjectFileURL(request);
        if (response.Succeeded)
        {
            result = true;
            _snackBar.Add(Localize["Project Updated"], MudBlazor.Severity.Success);
            _file = new AddEditProjectFileURLRequest();
            await UploadCompleted.InvokeAsync();
        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, MudBlazor.Severity.Error);

        return result;
    }

    public bool ModifiedForm()
    {
        return _editContext.IsModified();
    }
}