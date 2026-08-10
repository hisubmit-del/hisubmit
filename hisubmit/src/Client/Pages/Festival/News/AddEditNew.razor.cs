using System;
using System.IO;
using MudBlazor;
using AutoMapper;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Requests;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Shared.Components;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Components.Forms;
using HiSubmit.Client.Shared.Components.Base;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.News.Commands;
using HiSubmit.Client.Infrastructure.Managers.FestivalNews;
using HiSubmit.Client.Shared.Dialogs;

namespace HiSubmit.Client.Pages.Festival.News;

public partial class AddEditNew : BaseFestival
{
    #region Inject

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IFestivalNewsManager NewManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int NewId { get; set; }

    #endregion

    #region Private Field
    private bool _processing;
    private AddEditNewCommand _new = new();
    private CustomeRichTextEditor _richTextEditor;
    private FluentValidationValidator _fluentValidationValidator;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await CheckPermission(Permissions.FestivalNews.Edit);
        if (NewId != 0)
        {
            await LoadNew();
        }
        await base.OnInitializedAsync();
    }

    private async Task LoadNew()
    {
        var response = await NewManager.GetDetailAsync(
            new GetDetailNewQuery
            {
                Id = NewId
            }, SelectedFestivalId);

        if (response.Succeeded)
            _new = Mapper.Map<AddEditNewCommand>(response.Data);
        else
            foreach (var message in response.Messages)
                _snackBar.Add(message, Severity.Error);
    }

    private async Task SaveAsync()
    {
        _processing = true;
        _new.FestivalId = SelectedFestivalId;
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            var response = await NewManager.SaveAsync(_new, SelectedFestivalId);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                await PendingApproval();
                _navigationManager.NavigateTo("/festival/news");
            }
            else
                foreach (var message in response.Messages)
                    _snackBar.Add(message, Severity.Error);
        }
        _processing = false;
    }



    private async Task PendingApproval()
    {
        Console.WriteLine("show dialog in news");

        var options = new DialogOptions()
        {
            BackdropClick = true,
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };
        var d = await _dialogService.ShowAsync<ApprovedEmail>("Admin Approval", options);
        var res = await d.Result;
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
