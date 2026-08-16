using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System;
using System.Threading.Tasks;

namespace ClientComponents.Pages.Festival.FestivalEditComponent;

public partial class AddEventOrganizerModal
{
    [Inject]
    private IFestivalManager FestivalManager { get; set; }

    [Parameter]
    public AddEditEventOrginizerCommand Organizer { get; set; }
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }


    public bool Loaded { get; set; }

    private bool _processing;
        
    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
        Loaded = true;
    }

    public void Cancel()
    {
        MudDialog.Cancel();
    }


    private async Task SaveAsync()
    {
        _processing = true;
        if (Validated)
        {
            var response = await FestivalManager.SaveOrginizerAsync(Organizer);
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