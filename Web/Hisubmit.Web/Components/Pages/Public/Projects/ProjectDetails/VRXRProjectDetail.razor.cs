using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorVideoPlayer;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetVrXrSpecificationDetail;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class VRXRProjectDetail
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
        //await LoadSpecification();

        if (Files.Any(p => p.IsMainFile && p.IsLocalFile))
        {
            sources.Add(new Source() { Src = Files.FirstOrDefault(p => p.IsMainFile && p.IsLocalFile)!.LocalFileURL });
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