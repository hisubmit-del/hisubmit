using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class ProjectAwards
{
    #region Injection

    [Inject] public IProjectManager ProjectManager { get; set; }

    #endregion

    #region Parameter

    [Parameter] public int ProjectId { get; set; }
    [Parameter] public ProjectType ProjectType { get; set; }

    #endregion

    #region Private Filled

    private List<GetAwardDetailResponse> Awards { get; set; } = new();
    private List<GetScreenAwardResponse> ScreenAwards { get; set; } = new();

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await GetAllAwards();
        await GetAllScreenAward();
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
       
    }

    #endregion

    private async Task GetAllAwards()
    {
        var response = await ProjectManager.DetailAward(new GetAwardDetailRequest
        {
            ProjectId = ProjectId
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
            ProjectId = ProjectId
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
}