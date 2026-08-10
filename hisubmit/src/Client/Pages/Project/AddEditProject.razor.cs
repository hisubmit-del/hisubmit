using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.ReleaseProject;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using HiSubmit.Client.Shared.Dialogs;

namespace HiSubmit.Client.Pages.Project;

public partial class AddEditProject
{
    #region Parameter

    [Parameter] public int ProjectId { get; set; }

    [Parameter] public string TabName { get; set; }

    #endregion

    #region Inject

    [Inject] private IProjectManager ProjectManager { get; set; }

    #endregion

    #region Private Filled

    private int _activePanelIndex;
    private bool _releaseProcessing;
    private string _projectUrl;
    private MudTabs MainTab { get; set; }

    #endregion

    #region ChildComponentRef

    private AddEditProjectFile _addEditProjectFile;
    private AddEditProjectCredit _addEditProjectCredit;
    private AddEditProjectInformation _addEditProjectInformation;
    private AddEditProjectSpecification _addEditProjectSpecification;
    private ProjectAwardAndDistribution _projectAwardAndDistribution;

    #endregion

    protected override async Task OnInitializedAsync()
    {
        if (TabName == "info")
        {
           await ChangeTab(2);
        }
        await base.OnInitializedAsync();
    }

    private async Task ChangeTab(int index)
    {
        await Task.Run(() => { MainTab.ActivatePanel(index); });
    }

    private async Task ChangeTabChecked(int selectedTab)
    {
        if (selectedTab != _activePanelIndex)
        {
            switch (_activePanelIndex)
            {
                case 0:
                    if (_addEditProjectInformation.ModifiedForm())
                        await ShowNextAndSaveModal(selectedTab);
                    else
                        _activePanelIndex = selectedTab;
                    break;

                case 1:
                    if (_addEditProjectCredit.ModifiedForm())
                        await ShowNextAndSaveModal(selectedTab);
                    else
                        _activePanelIndex = selectedTab;
                    break;

                case 2:
                    if (_addEditProjectSpecification.ModifiedForm())
                        await ShowNextAndSaveModal(selectedTab);
                    else
                        _activePanelIndex = selectedTab;
                    break;
                case 3:
                    if (_projectAwardAndDistribution.ModifiedForm())
                        await ShowNextAndSaveModal(selectedTab);
                    else
                        _activePanelIndex = selectedTab;
                    break;
                case 4:
                    if (_addEditProjectFile.ModifiedForm())
                        await ShowNextAndSaveModal(selectedTab);
                    else
                        _activePanelIndex = selectedTab;
                    break;
            }
        }
    }

    private async Task ShowNextAndSaveModal(int selectedTab, bool saveButton = true)
    {
        var option = new DialogOptions
        {
            FullWidth = true,
            CloseButton = true,
            MaxWidth = MaxWidth.Small,
            
        };
        var parameters = new DialogParameters
        {
            { nameof(SaveAndNext.SaveButton), saveButton }
        };
        var dialog = _dialogService.Show<SaveAndNext>("Save or Next", parameters, option);
        var result = await dialog.Result;

        switch (result.Data.ToString())
        {
            case "SaveAndNext":
                switch (_activePanelIndex)
                {
                    case 0:
                        if (await _addEditProjectInformation.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                    case 1:
                        if (await _addEditProjectCredit.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                    case 2:
                        if (await _addEditProjectSpecification.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                    case 3:
                        if (await _projectAwardAndDistribution.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                    case 4:
                        if (await _addEditProjectFile.SaveAsync())
                            _activePanelIndex = selectedTab;
                        break;
                }

                break;
            case "Next":
                _activePanelIndex = selectedTab;
                break;
        }
    }

    private async Task UpdateProjectId(AddEditProjectDetailCommand command)
    {
        await Task.Run(() => { ProjectId = command.Id; });
        _projectUrl = command.URL;
    }

    private async Task ReleaseProject()
    {
        _releaseProcessing = true;
        var response = await ProjectManager.ReleaseProject(new ReleaseProjectCommand()
        {
            Id = ProjectId
        });
        if (response.Succeeded)
        {
            var parameters = new DialogParameters
                { { nameof(SuccessfullyProccess.ContentText), response.Messages[0] } };

            var options = new DialogOptions() { CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Small };

            _dialogService.Show<SuccessfullyProccess>("Success", parameters, options);
        }
        else
        {
            var parameter = new DialogParameters
            {
                { nameof(EmptyItemForReleaseFestival.Messages), response.Messages },
                { nameof(EmptyItemForReleaseFestival.Title), Localize["Project Cant be Released"].Value }
            };
            var options = new DialogOptions() { CloseButton = true, FullWidth = true, MaxWidth = MaxWidth.Small };
            _dialogService.Show<EmptyItemForReleaseFestival>("Empty Item", parameter, options);
        }

        _releaseProcessing = false;
    }

    private async Task GotoProjectPage()
    {
     _navigationManager.NavigateTo($"/project/{_projectUrl}");   
    }
}