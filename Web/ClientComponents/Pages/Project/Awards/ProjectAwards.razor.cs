using System;
using AutoMapper;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace ClientComponents.Pages.Project.Awards
{
    public partial class ProjectAwards
    {
        #region  Injection
        [Inject] private IProjectManager ProjectManager { get; set; }
        [Inject] private IMapper Mapper { get; set; }
        #endregion

        #region  Parameters
        [CascadingParameter] public int ProjectId { get; set; }
        #endregion
        
        private FluentValidationValidator _fluentValidationValidator;
        public UpdateAwardRequest Request=new();
        private bool Validated { get; set; }
        private bool _loaded ;

        private EditContext _editContext;
        protected override async Task OnInitializedAsync()
        {
            await LoadAwards();
            _editContext = new EditContext(Request);
            await base.OnInitializedAsync();
            _loaded = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
        }

        private async Task LoadAwards()
        {
            var response = await ProjectManager.DetailAward(new GetAwardDetailRequest()
            {
                ProjectId = ProjectId
            });

            if (response.Succeeded)
            {
                if (response.Data.Any())
                {
                    Request.Awards = Mapper.Map<List<AddEditAwardRequest>>(response.Data);
                }
                else
                {
                    Request.Awards = new List<AddEditAwardRequest>
                    {
                        new()
                        {
                            ProjectId = ProjectId,
                            Date = DateTime.Today
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

        private async Task DeleteAward(AddEditAwardRequest request)
        {
            await Task.Run(() => { Request.Awards.Remove(request); });
        }

        private async Task AddAward()
        {
            await Task.Run(() => { Request.Awards.Add(new AddEditAwardRequest {ProjectId = ProjectId}); });
        }

        public bool CheckValid()
        {
            Validated = _fluentValidationValidator.Validate((p) => p.IncludeAllRuleSets());
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
