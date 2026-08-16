using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Features.Submits.Commands;
using HiSubmit.Client.Infrastructure.Managers.Submits;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Web.Components.Pages.Festival.Submits
{
    public partial class FinalResultModal
    {
        [Inject]
        public ISubmitManager SubmitManager { get; set; }

        [Parameter] public List<int> SubmitId { get; set; } = new();

        [CascadingParameter]public IMudDialogInstance MudDialog { get; set; }

        private AddEditFinalJudgingCommand FinalJudging { get; set; }
        
        private  bool _processing { get; set; }

        protected override async Task OnInitializedAsync()
        {
            FinalJudging = new AddEditFinalJudgingCommand() { SubmitId = SubmitId };
            await base.OnInitializedAsync();
        }

        private async Task SubmitResult()
        {
            _processing = true;
            var response =await SubmitManager.FinalResult(FinalJudging);
            if (response.Succeeded)
            {
                _snackBar.Add(Localize["Submit status updated"] , Severity.Success) ;
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

        private async Task Cancel()
        {
            MudDialog.Close();
        }
    }
}
