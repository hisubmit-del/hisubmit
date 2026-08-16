using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.EditProjectSubmitterInformation;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System.Threading.Tasks;

namespace Web.Components.Pages.Project
{
    public partial class EditProjectSubmitterInformation
    {
        [CascadingParameter]
        public int ProjectId { get; set; }

        [Parameter]
        public EventCallback NextPanel { get; set; }


        [Inject]
        private IProjectManager ProjectManager { get; set; }

        [Inject]
        private IMapper Mapper { get; set; }

        private GetProjectDetailResponse _project { get; set; } = new();
        private EditProjectSubmitterInformationCommand _model { get; set; } = new();
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated { get; set; } = true;
        public bool Loaded { get; set; }


        private bool _processing { get; set; }
        public EditContext _EditForm { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            _EditForm = new EditContext(_model);
            await base.OnInitializedAsync();

            Loaded = true;
        }
        public async Task LoadData()
        {
            await LoadProjectInformatiion();
        }
        private async Task LoadProjectInformatiion()
        {
            if (ProjectId != 0)
            {
                var result = await ProjectManager.GetDetailAsync(new GetProjectDetailQuery
                {
                    Id = ProjectId
                });
                if (result.Succeeded)
                {
                    var project = result.Data;
                    _model = Mapper.Map<EditProjectSubmitterInformationCommand>(project);
                    if(_model.Address == null)
                    {
                        _model.Address = new AddEditAddressCommand() { ProjectId=ProjectId};
                    }
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private async Task<bool> SaveAsync()
        {

            Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
            if (Validated)
            {

                _processing = true;
                _model.Id = ProjectId;
                var result = await ProjectManager.UpdateSubmitterAsync(_model);
                _processing = false;
                if (result.Succeeded)
                {
                    _snackBar.Add(result.Messages[0], Severity.Success);
                    ProjectId = result.Data;
                    return true;
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
            return false;
        }
        private async Task GoNext()
        {
            if (await CheckForm())
            {
                await NextPanel.InvokeAsync();
            }
        }

        public async Task<bool> CheckForm()
        {
            if (_EditForm.IsModified())
            {
                var parameters = new DialogParameters();
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true,  };
                var dialog = _dialogService.Show<Shared.Dialogs.SaveAndNext>(localizer["Warning"], parameters, options);
                var result = await dialog.Result;
                if (!result.Canceled)
                {
                    if (!await SaveAsync())
                    {
                        return false;
                    }
                }
                return true;
            }
            return true;
        }
    }
}
