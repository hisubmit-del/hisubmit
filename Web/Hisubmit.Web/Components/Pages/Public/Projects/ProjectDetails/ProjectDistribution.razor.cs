using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.MediaRights.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using HiSubmit.Client.Infrastructure.Managers.MediaRights;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Pages.Public.Projects.ProjectDetails;

public partial class ProjectDistribution
{
    [Inject] private IProjectManager ProjectManager { get; set; }
    [Inject] private  IMediaRightManager MediaRightManager { get; set; }

    [Parameter] public int ProjectId { get; set; }

    private List<AddEditDistributionInformationRequest> _information = new();
    private List<GetAllMediaRightResponse> _mediaRights=new();
    protected override async Task OnInitializedAsync()
    {
        await LoadMediaRights();
        await LoadInformation();
        await base.OnInitializedAsync();
    }

    public async Task LoadInformation()
    {
        var response = await ProjectManager.DetailDistributionInformation(new GetAllDistribuationInformationQuery
        {
            ProjectId = ProjectId
        });
        if (response.Succeeded)
        {
            _information = response.Data;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }

    private async Task LoadMediaRights()
    {
        var response = await MediaRightManager.GetAllAsync(new GetAllMediaRightQuery());
        if (response.Succeeded)
        {
            _mediaRights = response.Data;
        }  else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, Severity.Error);
            }
        }
    }
    private  string GetMediaRightName(int id)
    {
        return _mediaRights
            .Where(p => p.Id == id)
            .Select(p => p.Name).FirstOrDefault();
    }
}