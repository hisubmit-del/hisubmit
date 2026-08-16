using System;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetDetail;
using HiSubmit.Client.Infrastructure.Managers.Projects;
using Hisubmit.Client.SharedModels.Enums;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using HiSubmit.Web.Components.Pages.Project.Specification;

namespace HiSubmit.Web.Components.Pages.Project
{
    public partial class AddEditProjectSpecification
    {
        #region Inject

        [Inject] private IProjectManager ProjectManager { get; set; }

        #endregion

        #region Parameters

        [CascadingParameter] public int ProjectId { get; set; }
        [Parameter] public EventCallback NextPanel { get; set; }

        #endregion

        #region Private Field

        private ProjectType ProjectType { get; set; }
        private bool _loaded = false;

        #endregion

        #region ChildComponent

        private FilmSpecification _filmSpecification;
        private MusicSpecification _musicSpecification;
        private PhotographySpecification _photographySpecification;
        private ScriptSpecification _scriptSpecification;
        private VrXrSpecification _vrXrSpecification;

        #endregion

        #region Override

        protected override async Task OnInitializedAsync()
        {
            await LoadProjectType();
            await base.OnInitializedAsync();
            _loaded = true;
        }

        #endregion

        private async Task GoNextPanel()
        {
            await NextPanel.InvokeAsync();
        }

        private async Task LoadProjectType()
        {
            var response = await ProjectManager.GetDetailAsync
                (new GetProjectDetailQuery() { Id = ProjectId });
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

        public bool ModifiedForm()
        {
            
            var modified = false;
            switch (ProjectType)
            {
                case ProjectType.Film:
                    if (_filmSpecification.ModifiedForm())
                        modified = true;
                    break;
                case ProjectType.Photography:
                    if (_photographySpecification.ModifiedForm())
                        modified = true;
                    break;
                case ProjectType.Music:
                    if (_musicSpecification.ModifiedForm())
                        modified = true;
                    break;
                case ProjectType.Script_ScreenWriting:
                    if (_scriptSpecification.ModifiedForm())
                        modified = true;
                    break;
                case ProjectType.VR_XR:
                    if (_vrXrSpecification.ModifiedForm())
                        modified = true;
                    break;
                case ProjectType.Art:
                    modified = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return modified;
        }

        public async Task<bool> SaveAsync()
        {
            var result = false;
            switch (ProjectType)
            {
                case ProjectType.Film:
                    if (await _filmSpecification.SaveAsync())
                        result = true;
                    break;
                case ProjectType.Photography:
                    if (await _photographySpecification.SaveAsync())
                        result = true;
                    break;
                case ProjectType.Music:
                    if (await _musicSpecification.SaveAsync())
                        result = true;
                    break;
                case ProjectType.Script_ScreenWriting:
                    if (await _scriptSpecification.SaveAsync())
                        result = true;
                    break;
                case ProjectType.VR_XR:
                    if (await _vrXrSpecification.SaveAsync())
                        result = true;
                    break;
                case ProjectType.Art:
                    result = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return result;
        }
    }
}