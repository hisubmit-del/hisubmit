using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Extensions;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor.Extensions;

namespace HiSubmit.Web.Components.Pages.Project.Files;

public partial class AddMusicFile
{
    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int ProjectId { get; set; }
    [Parameter] public EventCallback UploadCompleted { get; set; }
    [Parameter] public EventCallback Canceled { get; set; }

    #endregion

    #region Private Filled

    private AddEditProjectFileURLRequest Model { get; set; } = new() { IsLocalFile = true };
    private bool _processing;
    private IBrowserFile _file;
    private bool _uploading;
    private FluentValidationValidator _localFileValidator;
    private string _echo;
    private string _selectedFileName;
    private string _processingTitle;
    private long _percent;

    #endregion

    private Task AddToBrowserFile(InputFileChangeEventArgs e)
    {
        _file = e.File;
        _selectedFileName = _file.Name;
        return Task.CompletedTask;
    }

    private async Task Upload()
    {
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
                var response = await ProjectManager.UploadProjectFile(ProjectId, fragment++, formFile);
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
            Model.IsLocalFile = true;
            Model.LocalFileURL = dbPath;
            Model.ProjectId = ProjectId;
            Model.Id = 0;
            _echo = string.Empty;
            var validate = _localFileValidator.Validate((p) => p.IncludeAllRuleSets());
            if (validate)
            {
                await UpdateFileUrl(Model);
            }

            await UploadCompleted.InvokeAsync();
        }

        _processing = false;
        _processingTitle = string.Empty;
    }

    private async Task UpdateFileUrl(AddEditProjectFileURLRequest request)
    {
        request.ProjectId = ProjectId;
        var response = await ProjectManager.UpdateProjectFileURL(request);
        if (response.Succeeded)
        {
            _snackBar.Add(Localize["Project Updated"], MudBlazor.Severity.Success);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    private async Task Cancel()
    {
        await Canceled.InvokeAsync();
    }
}