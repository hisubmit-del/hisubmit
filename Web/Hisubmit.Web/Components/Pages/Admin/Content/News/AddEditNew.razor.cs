using System;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Client.Infrastructure.Managers.AdminNews;
using HiSubmit.Web.Components.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Admin.Content.News;

public partial class AddEditNew
{
    #region Inject

    [Inject]
    private  IAdminNewManager NewManager { get; set; }
    [Inject]
    private  IMapper Mapper { get; set; }

    #endregion

    #region Parameters

    [Parameter]
    public int NewId { get; set; }

    #endregion

    #region Private Field

    private AddEditNewCommand _new = new();
    private FluentValidationValidator _fluentValidationValidator;
    private CustomeRichTextEditor _richTextEditor;
    private bool _processing;

    #endregion
   

   

    protected override async Task OnInitializedAsync()
    {
        if (NewId != 0)
        {
           await LoadNew();
        }
        await base.OnInitializedAsync();
    }

    private async Task LoadNew()
    {
        var response = await NewManager.GetDetailAsync(new GetDetailNewQuery()
        {
            Id = NewId
        });
        if (response.Succeeded)
        {
            _new = Mapper.Map<AddEditNewCommand>(response.Data);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task SaveAsync()
    {
        _processing = true;

        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await NewManager.SaveAsync(_new);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/admin/news");
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
    
    
    
    #region manage reward logo file 
    private IBrowserFile _logoRewardFile;
    private async Task UploadBannerFileAsync(InputFileChangeEventArgs e)
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
            _new.BannerUrl = $"data:{format};base64,{Convert.ToBase64String(buffer)}";
            _new.UploadRequest = new UploadRequest()
            {
                Data = buffer,
                Extension = extension,
                UploadType = UploadType.NewBanner,
                FileName = fileName
            };
        }

    }
    private void DeleteBannerAsync()
    {
        _new.BannerUrl = string.Empty;
        _new.UploadRequest = new UploadRequest();
    }
    #endregion
}