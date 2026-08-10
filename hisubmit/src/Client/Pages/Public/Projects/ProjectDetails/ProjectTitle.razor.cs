using System;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Pages.Public.Projects.ProjectDetails;

public partial class ProjectTitle
{
    private bool _loaded;
    [Inject]
    private IProjectSpecificationManager ProjectSpecificationManager { get; set; }

    [Parameter] public GetProjectDetailResponse Project { get; set; } = new();


    [Parameter] public EventCallback<GetFilmSpecificationDetailResponse> FilmSpecificationChanged { get; set; }
    [Parameter] public GetFilmSpecificationDetailResponse FilmSpecification { get; set; }


    [Parameter] public GetPhotographySpecificationDetailResponse PhotographySpecification { get; set; } = new();
    [Parameter] public EventCallback<GetPhotographySpecificationDetailResponse> PhotographySpecificationChanged { get; set; }

    [Parameter] public GetMusicSpecificationDetailResponse MusicSpecification { get; set; }

    [Parameter] public EventCallback<GetMusicSpecificationDetailResponse> MusicSpecificationChanged { get; set; }

    [Parameter] public GetScriptSpecificationDetailResponse ScriptSpecification { get; set; }

    [Parameter] public EventCallback<GetScriptSpecificationDetailResponse> ScriptSpecificationChanged { get; set; }

    [Parameter] public EventCallback<GetVrXrSpecificationDetailResponse> VrXrSpecificationChanged { get; set; }
    [Parameter] public GetVrXrSpecificationDetailResponse VrXrSpecification { get; set; }


    protected override async Task OnInitializedAsync()
    {

        switch(Project.ProjectType)
        {
            case ProjectType.Film:
                await LoadFilmSpecification();
                break;
            case ProjectType.Photography:
                await LoadPhotographySpecification();
                break;
            case ProjectType.Music:
                await LoadMusicSpecification();
                break;
            case ProjectType.Script_ScreenWriting:
                await LoadScriptSpecification();
                break;
            case ProjectType.VR_XR:
                await LoadXrVrSpecification();
                break;
            case ProjectType.Art:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _loaded = true;
        await base.OnInitializedAsync();
    }

    private async Task LoadFilmSpecification()
    {
        var response = await ProjectSpecificationManager
            .GetFilmSpecification(new GetFilmSpecificationDetailRequest
            {
                ProjectId = Project.Id
            });

        if (response.Succeeded)
        {
            FilmSpecification = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        await FilmSpecificationChanged.InvokeAsync(FilmSpecification);
    }


    private async Task LoadMusicSpecification()
    {
        var response = await ProjectSpecificationManager.GetMusicSpecification
        (new GetMusicSpecificationDetailQuery
        {
            ProjectId = Project.Id
        });

        if (response.Succeeded)
        {
            MusicSpecification = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        await MusicSpecificationChanged.InvokeAsync(MusicSpecification);
    }

    private async Task LoadPhotographySpecification()
    {
        var response = await ProjectSpecificationManager.GetPhotographySpecification
        (new GetPhotographySpecificationDetailQuery
        {
            ProjectId = Project.Id
        });

        if (response.Succeeded)
        {
            PhotographySpecification = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        await PhotographySpecificationChanged.InvokeAsync(PhotographySpecification);
    }

    private async Task LoadScriptSpecification()
    {
        var response = await ProjectSpecificationManager
            .GetScriptSpecification(new GetScriptSpecificationDetailQuery
            {
                ProjectId = Project.Id
            });

        if (response.Succeeded)
        {
            ScriptSpecification = response.Data;
            await ScriptSpecificationChanged.InvokeAsync(ScriptSpecification);
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadXrVrSpecification()
    {
        var response = await ProjectSpecificationManager
            .GetXrVRSpecification(new GetVrXrSpecificationDetailQuery
            {
                ProjectId = Project.Id
            });

        if (response.Succeeded)
        {
            VrXrSpecification = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }

        await VrXrSpecificationChanged.InvokeAsync(VrXrSpecification);
    }
}