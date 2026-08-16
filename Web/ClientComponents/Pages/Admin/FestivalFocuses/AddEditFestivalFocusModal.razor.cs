using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Commands.AddEditFestivalFocus;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Catalog.FestivalFocus;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace ClientComponents.Pages.Admin.FestivalFocuses
{
    public partial class AddEditFestivalFocusModal
    {

        [Inject] private IFestivalFocusManager FestivalFocusManager { get; set; }

        [Parameter] public AddEditFestivalFocusCommand FestivalFocus { get; set; } = new();
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated { get; set; } = true;

        private bool _processing;
        public void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SaveAsync()
        {
            _processing = true;
            Validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

            var response = await FestivalFocusManager.SaveAsync(FestivalFocus);
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
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);

            _processing = false;
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}
