using MudBlazor;
using System.Linq;
using Hisubmit.Client.SharedModels.Enums;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Infrastructure.Managers.SubProjectTypes;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.SubProjectTypes.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectFiles;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;

namespace ClientComponents.Pages.Public.Projects.ProjectDetails;

public partial class ScriptProjectDetail
{
    #region Injection

    [Inject] private ISubProjectTypeManager SubProjectTypeManager { get; set; }

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
        //await LoadCategories();
        await base.OnInitializedAsync();
        _loaded = true;
    }

    #endregion

    //private async Task LoadCategories()
    //{
    //    var response = await SubProjectTypeManager.GetAllAsync(new GetAllSubProjectTypeQuery
    //    {
    //        ProjectType = ProjectType.Film,
    //        SubIdString = string.Join("-", Specification.SubProjectTypeIds)
    //    });
    //    if (response.Succeeded)
    //    {
    //        _subProjectNames = response.Data.Select(p => p.Name).ToList();
    //    }
    //    else
    //    {
    //        foreach (var message in response.Messages)
    //        {
    //            _snackBar.Add(message, Severity.Error);
    //        }
    //    }
    //}
   
    private async Task RateProject()
    {
        await RateProjectClicked.InvokeAsync();
    }
}