using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using Hisubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Festival.FestivalEditComponent;

public partial class AddEditEventVenueModal
{
    [Inject]
    private IFestivalManager FestivalManager { get; set; }

    [Parameter]
    public AddEditFestivalVenueCommand Venue { get; set; }
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated { get; set; }=true;
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }


    private bool _loaded;


    private bool _processing;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
        _loaded = true;
    }

    private void Cancel()
    {
        MudDialog.Cancel();
    }


    private async Task SaveAsync()
    {
        Validated= _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        _processing = true;
        if (Validated)
        {
            var response = await FestivalManager.SaveVenueAsync(Venue);
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
        }

        _processing = false;
    }

        
    private async Task LoadDataAsync()
    {
        await Task.CompletedTask;
    }
}