using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;
using Hisubmit.Client.SharedModels.Features.Permissions.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectImages;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetMusicSpecificationDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using ClientComponents.Pages.Project.Files;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace ClientComponents.Pages.Public.Projects.ProjectDetails;

public partial class PhotographyProjectDetail
{
    #region Injection

    [Inject] private IProjectManager ProjectManager { get; set; }
    [Inject] private IProjectSpecificationManager ProjectSpecificationManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public int ProjectId { get; set; }
    [Parameter] public List<GetAllProjectFileResponse> Files { get; set; }
    [Parameter] public GetProjectDetailResponse Project { get; set; }
    [CascadingParameter] public ProjectPermissionResponse Permission { get; set; }
    [Parameter] public bool DetailLoaded { get; set; }

  

    #endregion

    #region Private Filled

    private List<GetAllProjectImageResponse> _images = new();


    private bool _loaded;

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
     //   await GetAllImages();
      
        _loaded = true;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await _jsRuntime.InvokeVoidAsync("CreatePhotographicImageSlider",true);

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

  
}