using Blazored.FluentValidation;
using HiSubmit.Client.Extensions;
using HiSubmit.Client.SharedModels.Constants.Application;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FooterItems.Commands;
using HiSubmit.Client.Infrastructure.Managers.Footer;

namespace HiSubmit.Client.Pages.Admin.Content.News;

public partial class AddEditFooterItem
{
    #region Injection

    [Inject] private IFooterManager FooterManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public AddEditFooterItemCommand AddEditFooterItemModel { get; set; } = new();
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }        

    #endregion

    #region  Provate Filled
    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    private bool _processing ;

    #endregion

    #region Override
    protected override async Task OnInitializedAsync()
    {
        HubConnection = HubConnection.TryInitialize(_navigationManager);
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            await HubConnection.StartAsync();
        }
    }
    #endregion
    public void Cancel()
    {
        MudDialog.Cancel();
    }
    private async Task SaveAsync()
    {
        _processing = true;
        var response = await FooterManager.SaveAsync(AddEditFooterItemModel);
        if (response.Succeeded)
        {
            _snackBar.Add(response.Messages[0], Severity.Success);
            await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
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
}