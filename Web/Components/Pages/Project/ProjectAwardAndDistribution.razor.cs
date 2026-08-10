using AutoMapper;
using AutoMapper.Internal;
using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.Locatuions.Countries.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;
using HiSubmit.Client.Infrastructure.Managers.Locations;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Web.Components.Pages.Project.Awards;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Components.Pages.Project;

public partial class ProjectAwardAndDistribution
{
    #region Injection

    [Inject] private IMapper Mapper { get; set; }
    [Inject] private IProjectManager ProjectManager { get; set; }
    [Inject] private ILocationManager LocationManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public EventCallback NextPanel { get; set; }

    [CascadingParameter] public int ProjectId { get; set; }

    #endregion

    #region Private Filled

    private ProjectType ProjectType { get; set; }
    public List<AddEditDistributionInformationRequest> Distributions { get; set; }
    public List<AddEditAwardRequest> Awards { get; set; }
    public List<AddEditScreenWritingRequest> ScreenAwards { get; set; }
    private bool _loaded = false;
    private bool _processing = false;
    private ProjectAwards ProjectAwards { get; set; }
    private ProjectScreenAwards ProjectScreenAwards { get; set; }
    private ProjectDistribuationInformation ProjectDistributionInformation { get; set; }

    #endregion

    #region Override

    protected override async Task OnInitializedAsync()
    {
        await LoadProjectType();
        _loaded = true;
        await base.OnInitializedAsync();
    }

    #endregion

    private async Task LoadProjectType()
    {
        var response = await ProjectManager.GetDetailAsync(new GetProjectDetailQuery()
        {
            Id = ProjectId
        });

        if (response.Succeeded)
        {
            ProjectType = response.Data.ProjectType;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }

    public async Task<bool> SaveAsync()
    {
        _processing = true;
        if (ProjectType is ProjectType.Film)
        {
            if (ProjectScreenAwards.CheckValid() && ProjectDistributionInformation.CheckValid())
            {
                var projectScreenResponse = await ProjectManager.UpdateScreenAwards(
                    new UpdateScreenWritingRequest()
                    {
                        ProjectId = ProjectId,
                        ScreenWritings = ProjectScreenAwards.Request.ScreenWritings,
                    });

                var distributionInformation = await ProjectManager.UpdateDistributionInformation(
                    new UpdateDistributionInformationCommand()
                    {
                        ProjectId = ProjectId,
                        Information = ProjectDistributionInformation._command.Information,
                    });

                if (projectScreenResponse.Succeeded && distributionInformation.Succeeded)
                {
                    _snackBar.Add(Localize["Project updated"], MudBlazor.Severity.Success);
                    _processing = false;
                    ProjectScreenAwards.SetUnModifiedForm();
                    ProjectDistributionInformation.SetUnModifiedForm();
                    return true;
                }

                foreach (var item in distributionInformation.Messages)
                    _snackBar.Add(item, MudBlazor.Severity.Error);

                var successMessages = new List<string>();
                var errorMessages = new List<string>();

                if (projectScreenResponse.Succeeded)
                    successMessages = successMessages.Concat(projectScreenResponse.Messages).ToList();
                else
                    errorMessages = errorMessages.Concat(projectScreenResponse.Messages).ToList();
                
                if (distributionInformation.Succeeded)
                    successMessages = successMessages.Concat(distributionInformation.Messages).ToList();
                else
                    errorMessages = errorMessages.Concat(distributionInformation.Messages).ToList();
                
                
                foreach (var message in successMessages)
                    _snackBar.Add(message, MudBlazor.Severity.Success);
                foreach (var message in errorMessages)
                    _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
        else
        {
            if (ProjectAwards.CheckValid() && ProjectDistributionInformation.CheckValid())
            {
                var projectResponse = await ProjectManager.UpdateAwards(new UpdateAwardRequest()
                {
                    ProjectId = ProjectId,
                    Awards = ProjectAwards.Request.Awards,
                });
                

                var distributionInformation = await ProjectManager.UpdateDistributionInformation(
                    new UpdateDistributionInformationCommand()
                    {
                        ProjectId = ProjectId,
                        Information = ProjectDistributionInformation._command.Information,
                    });
                
                if (projectResponse.Succeeded && distributionInformation.Succeeded)
                {
                    _snackBar.Add(Localize["Project updated"], MudBlazor.Severity.Success);
                    _processing = false;
                    ProjectAwards.SetUnModifiedForm();
                    ProjectDistributionInformation.SetUnModifiedForm();
                    return true;
                }

                foreach (var item in distributionInformation.Messages)
                    _snackBar.Add(item, MudBlazor.Severity.Error);

                var successMessages = new List<string>();
                var errorMessages = new List<string>();

                if (projectResponse.Succeeded)
                    successMessages = successMessages.Concat(projectResponse.Messages).ToList();
                else
                    errorMessages = errorMessages.Concat(projectResponse.Messages).ToList();

                if (distributionInformation.Succeeded)
                    successMessages = successMessages.Concat(distributionInformation.Messages).ToList();
                else
                    errorMessages = errorMessages.Concat(distributionInformation.Messages).ToList();

                foreach (var message in successMessages)
                    _snackBar.Add(message, MudBlazor.Severity.Success);

                foreach (var message in errorMessages)
                    _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }

        _processing = false;
        return false;
    }

    public bool ModifiedForm()
    {
        if (ProjectType != ProjectType.Film)
            return ProjectAwards.ModifiedForm() || ProjectDistributionInformation.ModifiedForm();

        return ProjectScreenAwards.ModifiedForm() || ProjectDistributionInformation.ModifiedForm();
    }

    private async Task GoNext()
    {
        if (await SaveAsync())
        {
            await NextPanel.InvokeAsync();
        }
        else
        {
            _snackBar.Add(Localize["Error when saving forms"]);
        }
    }
}