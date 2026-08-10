using AdminDashboard.Wasm.Models;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.MediaRights.Queries;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllDistribuationInformationDetail;
using HiSubmit.Client.Infrastructure.Managers.MediaRights;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;

namespace HiSubmit.Client.Pages.Project.Awards
{
    public partial class ProjectDistribuationInformation
    {
        #region Injection
        [Inject]
        private IProjectManager ProjectManager { get; set; }
        #endregion

        #region Parameter
        [CascadingParameter]
        public int ProjectId { get; set; }
        #endregion



     //   public List<AddEditDistribuationInformationRequest> _model { get; set; } = new();
     public UpdateDistributionInformationCommand _command=new();

        public List<CheckBoxItem<int>> MediaRightItemRequest { get; set; }

        public FluentValidationValidator _fluentValidationValidator;

        public bool Validated { get; set; }
        public bool _Loaded = false;

        private EditContext _editContext;
        protected override async Task OnInitializedAsync()
        {
            await LoadInformation();
            _editContext = new EditContext(_command);
            await base.OnInitializedAsync();
            _Loaded = true;
        }

        private async Task LoadInformation()
        {
            var response = await ProjectManager.DetailDistributionInformation(new GetAllDistribuationInformationQuery()
            {
                ProjectId = ProjectId
            });

            if (response.Succeeded)
            {
                if (response.Data.Any())
                {
                    _command.Information = response.Data;
                }
                else
                {
                    _command.Information = new List<AddEditDistributionInformationRequest>
                    {
                        new()
                        {
                            ProjectId = ProjectId,
                            Items = new List<AddEditDistributionInformationItemRequest>
                            {
                                new()
                            }
                        }
                    };
                }
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, MudBlazor.Severity.Error);
                }
            }
        }


        private async Task DeleteAward(AddEditDistributionInformationRequest request)
        {
            await Task.Run(() => { _command.Information.Remove(request); });
        }
        private async Task AddAward()
        {
            await Task.Run(() => { _command.Information.Add(new AddEditDistributionInformationRequest{ ProjectId=ProjectId}); });
        }

        private async Task DeleteItem(AddEditDistributionInformationRequest information , AddEditDistributionInformationItemRequest item)
        {
            await Task.Run(() =>
            {
                information.Items.Remove(item);
            });
        }

        private async Task AddItem(AddEditDistributionInformationRequest information)
        {
            await Task.Run(() =>
            {
                information.Items.Add(new AddEditDistributionInformationItemRequest{ DistributionInformationId=information.Id});
            });
        }

        public bool CheckValid()
        {
            Validated = _fluentValidationValidator.Validate(p => p.IncludeAllRuleSets());

            return Validated;
        }
        public void SetUnModifiedForm()
        {
            _editContext.MarkAsUnmodified();
        }

        public bool ModifiedForm()
        {
            return _editContext.IsModified();
        }
    }
}
