using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Web.Components.Pages.Project;
using HiSubmit.Client.SharedModels.Extensions;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class AddHeaderFilesModal
{
    [Parameter] public List<GetAllProjectFileResponse> HeaderFiles { get; set; } = new();


    [Parameter]
    public AddEditProjectFileURLRequest ProjectFile { get; set; }

    [CascadingParameter] public IMudDialogInstance DialogInstance { get; set; }


    [Parameter]
    public bool IsLocalFile { get; set; }

    [Parameter]
    public bool ShowForm { get; set; }



    [Inject] public IProjectManager ProjectManager { get; set; }

    public string _acceptFile;
    public string _selectedFileName;

    private FluentValidationValidator _fluentValidationValidator;

    private bool _processing;
    private IBrowserFile _file;
    private string _processingTitle;
    private bool _uploading;
    private long _percent;
    private string _echo;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
    }

    private async Task ChangeShowForm()
    {
        ShowForm = true;
    }



    private Task AddToBrowserFile(InputFileChangeEventArgs e)
    {
        _file = e.File;
        _selectedFileName = ProjectFile.Name;
        return Task.CompletedTask;
    }

    public async Task SaveAsync()
    {
        //var validated = _fluentValidationValidator.Validate(opt => opt.IncludeAllRuleSets());
        //if (!validated)
        //    return;
        if (IsLocalFile)
        {
            var result = false;
            _processing = true;
            _processingTitle = "Uploading";
            const long cSize = 1024 * 400;
            var file = _file;
            long uploadedBytes = 0;
            var totalBytes = file.Size;
            var fragment = 0;
            var uploadCompleted = false;
            var uniqFileName = $"{DateTime.Now.ToFileTime()}_{Guid.NewGuid()}_{file.Name}";

            await using (var inStream = file.OpenReadStream(long.MaxValue))
            {
                _uploading = true;
                while (_uploading)
                {
                    var chunkSize = cSize;
                    if (uploadedBytes + cSize > totalBytes)
                    {
                        chunkSize = totalBytes - uploadedBytes;
                    }

                    var chunk = new byte[chunkSize];
                    var readAsync = await inStream.ReadAsync(chunk);
                    using var formFile = new MultipartFormDataContent();
                    var fileContent = new StreamContent(new MemoryStream(chunk));
                    formFile.Add(fileContent, "file", uniqFileName);
                    
                    var response = await ProjectManager.UploadProjectFile(ProjectFile.ProjectId, fragment++, formFile);
                    if (!response.Succeeded)
                    {
                        foreach (var item in response.Messages)
                        {
                            _snackBar.Add(item);
                        }

                        break;
                    }
                    uploadedBytes += chunkSize;
                    _percent = uploadedBytes * 100 / totalBytes;
                    _echo = $"Uploaded {_percent}%  {uploadedBytes / 1024} of {totalBytes / 1024}";
                    if (_percent >= 100)
                    {
                        uploadCompleted = true;
                        _uploading = false;
                    }
                    await InvokeAsync(StateHasChanged);
                }
            }

            _processingTitle = "Saving";

            if (uploadCompleted)
            {
                var folder = UploadType.ProjectFile.ToDescriptionString();
                var folderName = Path.Combine("Files", folder);
                var dbPath = Path.Combine(folderName, uniqFileName);

                ProjectFile.IsLocalFile = true;
                ProjectFile.LocalFileURL = dbPath;
                ProjectFile.Id = 0;

                _echo = string.Empty;
                var validate = await _fluentValidationValidator.ValidateAsync((p) => p.IncludeAllRuleSets());
                if (validate)
                {
                    await UpdateFileUrl(ProjectFile);
                }

            }

            _processing = false;
            _processingTitle = string.Empty;

        }
        else
        {
            //ProjectFile.Position = ProjectFilePosition.Header;
            ProjectFile.IsLocalFile = false;

            await UpdateFileUrl(ProjectFile);
        }
    }

    private async Task UpdateFileUrl(AddEditProjectFileURLRequest request)
    {
        var result = false;

        var response = await ProjectManager.UpdateProjectFileURL(request);
        if (response.Succeeded)
        {
            result = true;
            _snackBar.Add(Localize["Project Updated"], MudBlazor.Severity.Success);
            DialogInstance.Close();
        }
        else if (response.Data is { HasConflictFile: true })
        {
            var options = new DialogOptions()
            {
                FullWidth = true,
                MaxWidth = MaxWidth.Small,

            };
            var di = await _dialogService.ShowAsync<ProjectFileConflictWays>("title", options);
            var res = await di.Result;
            if (res != DialogResult.Cancel())
            {
                request.ConflictWays = (ConflictWays)res.Data;
                var result2 = await ProjectManager.UpdateProjectFileURL(request);
                if (result2.Succeeded)
                {
                    _snackBar.Add("Project file updated", Severity.Success);
                    DialogInstance.Close();
                }
                else
                    foreach (var message in result2.Messages)
                        _snackBar.Add(message, MudBlazor.Severity.Error);
            }

        }
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, MudBlazor.Severity.Error);
    }

    private void ChangeAcceptFile(FileFormat format)
    {
        ProjectFile.FileFormat = format;
        _acceptFile = format.GetAcceptedFormat();
    }

    private async Task Cancel(MouseEventArgs arg)
    {
        DialogInstance.Cancel();
    }
}


