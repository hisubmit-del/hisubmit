using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Permissions.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectImages;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.SubProjectTypes;
using Web.Components.Pages.Project.Files;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using BlazorVideoPlayer;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllSubProjectType;

namespace Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class MusicProjectDetail
{
    #region Injection

    [Inject] private ISubProjectTypeManager SubProjectTypeManager { get; set; }

    [Inject] private IProjectSpecificationManager ProjectSpecificationManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public List<GetAllProjectFileResponse> Files { get; set; }
    [Parameter] public GetProjectDetailResponse Project { get; set; }
    [Parameter] public EventCallback RateProjectClicked { get; set; }
    [Parameter] public ProjectPermissionResponse Permission { get; set; }
    [Parameter] public bool DetailLoaded { get; set; }
   

    #endregion

    #region Private Filled

    
    private List<string> _subProjectNames = new();
    private GetAllProjectFileResponse _music;
    private bool _loaded;
    private List<GetAllSubProjectTypeResponse> _subProjectTypes;
    private Player _player;
    #endregion

    #region override

    protected override async Task OnInitializedAsync()
    {
        _music = Files.First();
        
        sources = Files.Select(p => new Source()
        {
            Src = p.LocalFileURL
            //ItemType = p.FileFormat
        }).ToList();
        // _tracks= Files.Where(p => p.IsMainFile).Select(p => new Track()
        // {
        //     Src = p.LocalFileURL,
        //     Label = p.Name,
        //     
        //     //ItemType = p.FileFormat
        // }).ToList();
      

        await base.OnInitializedAsync();
        _loaded = true;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            await _jsRuntime.InvokeVoidAsync("CreateMusicPlayerSlider");

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

   

    private async Task RateProject()
    {
        await RateProjectClicked.InvokeAsync();
    }

    private  Task AddMusic()
    {
        var option = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            CloseOnEscapeKey = true,
            MaxWidth = MaxWidth.Small,
           
        };
        var parameters = new DialogParameters
        {
            { nameof(AddMusicFile.ProjectId), Project.Id }
        };
        _dialogService.Show(typeof(AddMusicFile), "Add Music", parameters, option);
        return Task.CompletedTask;
    }

    private void ChangeMusic(GetAllProjectFileResponse item)
    {
        _music = item;
       // _video.Src = item.LocalFileURL;
      
    }


    private void PlaySelectMusic(GetAllProjectFileResponse file)
    {
        sources.Clear();
        sources.Add(new Source()
        {
            Src = file.LocalFileURL,
            Type = "mp3"
        });
        _player.Sources = sources;
    }

    private List<Source> sources = new();
    private List<Track> _tracks = new();
    private void OnEndedVideo()
    {
    }

    private void OnPlayVideo()
    {
    }

    private void OnVideoTimeUpdate(float currentTime, float duration)
    {
    }
}