using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Client.Infrastructure.Managers.FestivalFiles;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;

namespace Web.Components.Pages.Festival.FestivalEditComponent;

public partial class AddEditFestivalFileModal
{
    [Inject] private IFestivalFileManager FestivalFileManager { get; set; }

    [Parameter] public AddEditFestivalFileCommand  File { get; set; } = new();
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; } =true;

    public void Cancel()
    {
        MudDialog.Cancel();
    }
        
    private  bool _processing { get; set; }

    private async Task SaveAsync()
    {
        Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        if (Validated)
        {
            _processing = true;
                
            var response = await FestivalFileManager.UpdateAsync(File,File.FestivalId);
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

            _processing = false;
        }
           
    }

    private IBrowserFile _file;

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var buffer = new byte[_file.Size];
            var extension = Path.GetExtension(_file.Name);
            var format = "application/octet-stream";
            await _file.OpenReadStream(_file.Size).ReadAsync(buffer);
            File.FileURL = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            File.UploadFileRequest = new UploadRequest { Data = buffer,FileName=_file.Name, UploadType = UploadType.Document, Extension = extension };
        }
    }
}