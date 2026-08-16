using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;

namespace Web.Components.Pages.Project;

public partial class AddEditProjectInformation
{
    #region Parameter

    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public EventCallback<AddEditProjectDetailCommand> UpdatedProjectId { get; set; }

    [CascadingParameter] public int ProjectId { get; set; }

    #endregion

    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }

    [Inject] private IMapper Mapper { get; set; }

    #endregion

    #region Private Filled

    private GetProjectDetailResponse Project { get; set; } = new();
    private AddEditProjectDetailCommand Modal { get; set; } = new();
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; } = true;
    private bool _loaded;
    private bool _processing;
    private EditContext EditForm { get; set; }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
        await UpdatedProjectId.InvokeAsync(Modal);
        EditForm = new EditContext(Modal);
        await base.OnInitializedAsync();
        _loaded = true;
        
    }

    #endregion

    #region Upload File

    private IBrowserFile _logoRewardFile;

    private async Task UploadRewardLogoFileAsync(InputFileChangeEventArgs e)
    {
        _logoRewardFile = e.File;
        if (_logoRewardFile != null)
        {
            var extension = Path.GetExtension(_logoRewardFile.Name);
            var format = "image/png";
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            Modal.StudentPhotoCard = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            Modal.UploadRequest = new UploadRequest()
            {
                Data = buffer,
                Extension = extension,
                UploadType = UploadType.UniversityCard,
                FileName = fileName
            };
        }
    }

    private void DeleteRewardLogoFileAsync()
    {
        Modal.StudentPhotoCard = string.Empty;
        Modal.UploadRequest = new UploadRequest();
    }

    #endregion

    private async Task LoadData()
    {
        await LoadProjectInformation();
    }

    private async Task LoadProjectInformation()
    {
        if (ProjectId != 0)
        {
            var result = await ProjectManager.GetDetailAsync(new GetProjectDetailQuery
            {
                Id = ProjectId
            });
            if (result.Succeeded)
            {
                var project = result.Data;
                Modal = Mapper.Map<AddEditProjectDetailCommand>(project);
                if (Modal.Address == null)
                {
                    Modal.Address = new AddEditAddressCommand() { ProjectId = ProjectId };
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
        else
        {
            var user = await AuthenticationManager.CurrentUser();
            Modal.Email = user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            Modal.FirstName=user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Name)?.Value;
            Modal.LastName=user.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Surname)?.Value;
        }
    }

    public async Task<bool> SaveAsync()
    {
        Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        if (Validated)
        {
            _processing = true;
            Modal.Id = ProjectId;
            var result = await ProjectManager.UpdateDetailAsync(Modal);
            _processing = false;
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                EditForm.MarkAsUnmodified();
                ProjectId = result.Data;
                Modal.Id = result.Data;
                await UpdatedProjectId.InvokeAsync(Modal);
                return true;
            }

            foreach (var message in result.Messages)
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

    public bool ModifiedForm()
    {
        return EditForm.IsModified();
    }
}