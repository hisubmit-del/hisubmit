using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Judgings.Commands.AddEditJudgiingButton;
using HiSubmit.Web.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Judgings;
using Hisubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Festival.Judging;

public partial class AddJudgingFiled
{
    #region Inject
    [Inject] private IJudgingManager JudgingManager { get; set; }

    #endregion

    #region Parametes

    [Parameter] public int FestivalId { get; set; }
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [Parameter] public AddEditJudgingFiledCommand JudgingFiledModal { get; set; } = new();

    #endregion
    
    private bool _processing;
    private bool _validated = true;
    private FluentValidationValidator _fluentValidationValidator;
    
    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        _processing = true;
        _validated = _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });

        var response = await JudgingManager.AddFiled(JudgingFiledModal,FestivalId);
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