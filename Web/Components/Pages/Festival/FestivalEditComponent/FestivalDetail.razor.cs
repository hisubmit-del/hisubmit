using AutoMapper;
using Blazored.FluentValidation;
using Blazored.LocalStorage;
using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Requests;
using HiSubmit.Client.Infrastructure.Managers.FestivalQualifires;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Components.Shared.Components;
using Hisubmit.Client.SharedModels.Enums;

namespace Web.Components.Pages.Festival.FestivalEditComponent;

public partial class FestivalDetail
{
    #region Inject

    [Inject] private IFestivalManager FestivalManager { get; set; }
    [Inject] private IMapper Mapper { get; set; }
    [Inject] private ILocalStorageService localStorageService { get; set; }

    [Inject] private IFestivalQualifiersManager FestivalQualifiersManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int FestivalId { get; set; }
    [Parameter] public EventCallback NextPanel { get; set; }
    [Parameter] public bool IsAdmin { get; set; }

    #endregion


    public GetFestivalDetailResponse _festival { get; set; } = new();
    public List<GetAllEventOrganizerResponse> _EventOrginizer { get; set; } = new();
    public AddEditFestivalDetailCommand _model { get; set; } = new();
    public List<GetAllFestivalQualifiersResponse> Qualifiers { get; set; } = new();

    public IEnumerable<string> QualifyersSelectedId = new List<string>();

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; } = true;
    public bool Loaded { get; set; }


    private bool _processing { get; set; }
    public EditContext _EditForm { get; set; }
    public string Qualifyer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        _model.UploadRequest = new UploadRequest();
        _model.RewardLogoUploadRequest = new UploadRequest();
        // await LoadFestivalId();
        await LoadData();
        _EditForm = new EditContext(_model);
        await base.OnInitializedAsync();

        Loaded = true;
    }

    public async Task LoadData()
    {
        await LoadQualifiers();
        await LoadFestivalData();
    }


    //private async Task LoadFestivalId()
    //{
    //   FestivalId = await localStorageService.GetItemAsync<int>(StorageConstants.Local.FestivalId);
    //}
    private async Task LoadQualifiers()
    {
        var response =
            await FestivalQualifiersManager.GetAllAsync(new GetAllFestivalQualifiersQuery());
        if (response.Succeeded)
        {
            Qualifiers = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadFestivalData()
    {
        var result = await FestivalManager.GetFestivalDetailAsync(new GetFestivalDetailByIdQuery
        {
            FestivalId = FestivalId,
            WithInclude = true
        });
        if (result.Succeeded)
        {
            var festival = result.Data;
            _model = Mapper.Map<AddEditFestivalDetailCommand>(festival);
            _model.UploadRequest ??= new UploadRequest();
            _model.RewardLogoUploadRequest ??= new UploadRequest();
            _model.ApprovedLicenseUploadRequest ??= new UploadRequest();
            //  Console.WriteLine(_model.QualifyersId.Count);
            _model.QualifyersId ??= new List<string>();
            QualifyersSelectedId = _model.QualifyersId.ToList();
        }
        else
        {
            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    public async Task<bool> SaveAsync()
    {
        if (_model.FestivalStatus == FestivalStatus.UnderInvestigation)
            return true;

        if (!IsAdmin)
        {
            if (string.IsNullOrWhiteSpace(_model.LogoURL) &&
                !(_model.UploadRequest?.Data is { Length: > 0 }))
            {
                _snackBar.Add("A festival logo is required.", Severity.Error);
                return false;
            }

            if (!_model.FilmFestival &&
                !_model.ScreenWritingWriter &&
                !_model.MusicContest &&
                !_model.PhotographicContest &&
                !_model.ArtFestival &&
                !_model.OnlineFestival)
            {
                _snackBar.Add("Select at least one festival type.", Severity.Error);
                return false;
            }
        }

        Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        if (Validated)
        {
            _processing = true;
            _model.QualifyersId = QualifyersSelectedId.ToList();
            _model.Id = FestivalId;
            var result = await FestivalManager.SaveDetailAsync(_model);
            _processing = false;
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                _EditForm.MarkAsUnmodified();
                FestivalId = result.Data;
                return true;
            }

            foreach (var message in result.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        await ScrollService.ScrollToId("fes-from");
        return false;
    }

    #region manage approved license  file

    private IBrowserFile _approvedLicenseFile;
    private CustomeRichTextEditor _richTextEditor;

    private async Task UploadApprovedLicenseFileAsync(InputFileChangeEventArgs e)
    {
        _approvedLicenseFile = e.File;
        if (_approvedLicenseFile != null)
        {
            var extension = Path.GetExtension(_approvedLicenseFile.Name);
            var format = "image/png";
            var fileName = $"{Guid.NewGuid()}{extension}";
            var imageFile = await e.File.RequestImageFileAsync(format, 400, 400);
            var buffer = new byte[imageFile.Size];
            await imageFile.OpenReadStream().ReadAsync(buffer);
            _model.ApprovedLicenseURL = _approvedLicenseFile.Name;
            _model.ApprovedLicenseUploadRequest = new UploadRequest()
            {
                Data = buffer,
                Extension = extension,
                UploadType = UploadType.ApprovedLicense,
                FileName = fileName
            };
        }
    }

    private void DeleteApprovedLicenseFileAsync()
    {
        _model.LogoURL = string.Empty;
        _model.UploadRequest = new UploadRequest();
    }

    #endregion

    private async Task GoNext()
    {
        await NextPanel.InvokeAsync();
    }


    public bool ModifiedForm()
    {
        return _EditForm.IsModified();
    }
}
