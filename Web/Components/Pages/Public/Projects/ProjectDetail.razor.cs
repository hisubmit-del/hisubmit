using System;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Linq;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Permissions.Queries;
using HiSubmit.Client.Infrastructure.Managers.CheckPermissions;
using Web.Components.Pages.Public.Projects.ProjectDetails;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UploadProjectFile;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Microsoft.AspNetCore.Components.Web;

namespace Web.Components.Pages.Public.Projects;

public partial class ProjectDetail
{
    #region Injection

    [Inject] public IProjectManager ProjectManager { get; set; }
    [Inject] private ICheckPermissionManager CheckPermissionManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public string ProjectUrl { get; set; }

    #endregion

    #region Private Property

    private GetProjectDetailResponse Project { get; set; }
    private List<GetAllProjectCreditResponse> Credits { get; set; } = new();
    private List<GetAllProjectFileResponse> Files { get; set; } = new();
    private bool _loaded;
    private ProjectPermissionResponse _permissions = ProjectPermissionResponse.Read;
    private bool _loadedProjectDetail;
    private bool _projectNotFound;
    private List<GetAllProjectFileResponse> _headerFiles = new();
    #endregion


    #region FilmSpecification

    public GetFilmSpecificationDetailResponse FilmSpecification { get; set; }
    public GetMusicSpecificationDetailResponse MusicSpecification { get; set; }
    public GetPhotographySpecificationDetailResponse PhotographySpecification { get; set; }
    public GetScriptSpecificationDetailResponse ScriptSpecification { get; set; }
    public GetVrXrSpecificationDetailResponse VrXrSpecification { get; set; }

    #endregion

    #region override

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectDetail();
        if (Project is null)
        {
            _projectNotFound = true;
            _loadedProjectDetail = true;
            await base.OnInitializedAsync();
            return;
        }

        await GetAllScreenAward();
        await GetAllAwards();
        _loadedProjectDetail = true;
        await base.OnInitializedAsync();
        await LoadFiles();
        _loaded = true;
        await GetUserPermission();
        await LoadProjectCredits();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        await _jsRuntime.InvokeVoidAsync("CreateImageProjectSlider");
        await _jsRuntime.InvokeVoidAsync("createAwardProjectSlider");
    }

    #endregion

    private async Task LoadProjectDetail()
    {
        var response = await ProjectManager.GetDetailAsync(new GetProjectDetailQuery
        {
            URL = ProjectUrl
        });
        if (response.Succeeded)
        {
            Project = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
            _projectNotFound = true;
        }
    }

    private async Task LoadProjectCredits()
    {
        if (Project == null)
            return;
        var response = await ProjectManager.GetAllProjectCreditAsync(new GetAllProjectCreditQuery()
        {
            ProjectId = Project.Id,
            WithInclude = true
        });
        if (response.Succeeded)
        {
            Credits = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadFiles()
    {
        var response = await ProjectManager.GetAllFiles(new GetAllProjectFilesQuery
        {
            ProjectId = Project.Id,
        });
        if (response.Succeeded)
        {
            Files = response.Data;

            _headerFiles = Files.Where(p => p.Position == ProjectFilePosition.Header).ToList();
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task GetUserPermission()
    {
        var response = 
            await CheckPermissionManager.CheckPermissionProject(new CheckProjectPermissionQuery
        {
            ProjectId = Project.Id
        });

        if (response.Succeeded)
        {
            _permissions = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }


    #region Awards

    private List<GetAwardDetailResponse> Awards { get; set; } = new();
    private List<GetScreenAwardResponse> ScreenAwards { get; set; } = new();

    private async Task GetAllAwards()
    {
        var response = await ProjectManager.DetailAward(new GetAwardDetailRequest
        {
            ProjectId = Project.Id
        });
        if (response.Succeeded)
        {
            Awards = response.Data;
        }

        foreach (var message in response.Messages)
        {
            _snackBar.Add(message, Severity.Error);
        }
    }

    private async Task GetAllScreenAward()
    {
        var response = await ProjectManager.DetailScreenAward(new GetScreenAwardRequest
        {
            ProjectId = Project.Id
        });
        if (response.Succeeded)
        {
            ScreenAwards = response.Data;
        }

        foreach (var message in response.Messages)
        {
            _snackBar.Add(message, Severity.Error);
        }
    }

    #endregion

    private async Task AddEditHeaderFiles()
    {
        var parameters = new DialogParameters<AddHeaderFilesModal>
        {
        { p=>p.ProjectFile, new AddEditProjectFileURLRequest()
        {
            Position = ProjectFilePosition.Header,
            ProjectId = Project.Id,
        } }
        };
        await ShowFileModal(parameters);
    }


    private async Task AddEditGallery()
    {
        var parameters = new DialogParameters<AddHeaderFilesModal>
        {
            {
                p => p.ProjectFile, new AddEditProjectFileURLRequest()
                {
                    Position = ProjectFilePosition.Gallery,
                    ProjectId = Project.Id,
                    FileFormat = FileFormat.Image,
                    IsLocalFile = true,
                }
            },
            { p => p.IsLocalFile, true },
            { p=>p.ShowForm, true }
        };

        await ShowFileModal(parameters);

    }

    private async Task AddFiles()
    {
        var parameters = new DialogParameters<AddHeaderFilesModal>
        {
            {
                p => p.ProjectFile, new AddEditProjectFileURLRequest()
                {
                    Position = ProjectFilePosition.SideBarFile,
                    ProjectId = Project.Id,
                    IsLocalFile = true,
                }
            },
            { p => p.IsLocalFile, true },
            { p => p.ShowForm, true }
        };

        await ShowFileModal(parameters);
    }

    private async Task AddExternalFiles()
    {
        var parameters = new DialogParameters<AddHeaderFilesModal>
        {
            {
                p => p.ProjectFile, new AddEditProjectFileURLRequest()
                {
                    Position = ProjectFilePosition.SideBarFile,
                    ProjectId = Project.Id,
                    IsLocalFile = true,
                }
            },
            { p => p.IsLocalFile, false },
            { p => p.ShowForm, true }
        };

        await ShowFileModal(parameters);
    }

    private async Task ShowFileModal(DialogParameters<AddHeaderFilesModal> parameters)
    {
        var options = new DialogOptions()
        {
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small,
        };

        var dialog = await _dialogService.ShowAsync<AddHeaderFilesModal>("Update Image", parameters, options);
        var res = await dialog.Result;
        if (!res.Canceled)
        {
            await LoadFiles();
            StateHasChanged();
            await _jsRuntime.InvokeVoidAsync("CreatePhotographicImageSlider");
            await _jsRuntime.InvokeVoidAsync("CreateImageProjectSlider");
            await _jsRuntime.InvokeVoidAsync("createAwardProjectSlider");
        }
    }
}
