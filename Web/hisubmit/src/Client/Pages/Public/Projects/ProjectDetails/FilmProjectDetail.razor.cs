using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorVideoPlayer;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.SubProjectTypes;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace HiSubmit.Client.Pages.Public.Projects.ProjectDetails;

public partial class FilmProjectDetail
{
    #region Injection
    [Inject] public IProjectManager ProjectManager { get; set; }
    [Inject] private IProjectSpecificationManager ProjectSpecificationManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public List<GetAllProjectFileResponse> Files { get; set; }
    [Parameter] public GetProjectDetailResponse Project { get; set; }
    [Parameter] public EventCallback RateProjectClicked { get; set; }
    [Parameter] public bool DetailLoaded { get; set; }
    

    #endregion

    #region Private Filled

    private List<string> _subProjectNames = new();
    private bool _loaded;

    #endregion

    #region override

    protected override async Task OnInitializedAsync()
    {
        if (Files.Any(p => p.IsLocalFile))
        {
            Console.WriteLine($"Player source:{Files.FirstOrDefault(p => p.IsLocalFile)!.LocalFileURL}");
            sources.Add(new Source() { Src = Files.FirstOrDefault(p =>  p.IsLocalFile)!.LocalFileURL });
        }

        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

   

   
    private async Task RateProject()
    {
        await RateProjectClicked.InvokeAsync();
    }
    

    private List<Source> sources = new();

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