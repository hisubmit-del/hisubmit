using System;
using System.IO;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Client.Infrastructure.Managers.AdminAdvertise;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace ClientComponents.Pages.Admin.Advertise;

[Authorize(Policy=Permissions.Advertise.BannerUpdate)]
public partial class AddAdvertiseBanner
{
    #region Inject

    [Inject] private IAdminAdvertiseManager AdvertiseManager { get; set; }

    #endregion

    #region Parameter

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [Parameter] public AddEditAdvertiseBannerRequest Model { get; set; } = new();

    #endregion

    #region Private Field

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    private bool _processing;

    #endregion

    private void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        _processing = true;
        var response = await AdvertiseManager.AddAdvertiseBanner(Model);
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

    private void DeleteAsync()
    {
        Model.Url = null;
        Model.UploadRequest = new UploadRequest();
    }

    private IBrowserFile _file;

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        _file = e.File;
        if (_file != null)
        {
            var extension = Path.GetExtension(_file.Name);
            var format = "image/png";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            Model.Url = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            Model.UploadRequest = new UploadRequest
                { Data = buffer, UploadType = UploadType.Advertise, Extension = extension, FileName = e.File.Name };
        }
    }
}