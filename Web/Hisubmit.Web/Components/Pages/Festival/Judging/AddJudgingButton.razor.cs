using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using HiSubmit.Client.Infrastructure.Managers.Judgings;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgingButton;

namespace HiSubmit.Web.Components.Pages.Festival.Judging
{
    public partial class AddJudgingButton
    {
        #region Inject
        
        [Inject] private IJudgingManager JudgingManager { get; set; }

        #endregion

        #region Parameters

        [Parameter] public int FestivalId { get; set; }
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [Parameter] public AddEditJudgingButtonCommand JudgingButtonModel { get; set; } = new();

        #endregion

        private bool _processing;
        private bool Validated { get; set; } = true;
        private FluentValidationValidator _fluentValidationValidator;

        public void Cancel()
        {
            MudDialog.Cancel();
        }
        private async Task SaveAsync()
        {
            _processing = true;
            Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

            var response = await JudgingManager.AddButton(JudgingButtonModel, FestivalId);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                MudDialog.Close();
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }

            _processing = false;
        }
        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
        }
    }
}
