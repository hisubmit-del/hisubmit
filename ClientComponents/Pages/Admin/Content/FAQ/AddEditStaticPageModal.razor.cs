using System.Threading.Tasks;
using Blazored.FluentValidation;
using ClientComponents.Extensions;
using HiSubmit.Client.Infrastructure.Managers.Footer;
using HiSubmit.Client.Infrastructure.Managers.StaticPages;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Features.FooterItems.Commands;
using Hisubmit.Client.SharedModels.Features.StaticPages.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;

namespace ClientComponents.Pages.Admin.Content.FAQ;

public partial class AddEditStaticPageModal
{
    #region Injection

    [Inject] private IStaticPageManager StaticPageManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public AddEditStaticPageRequest Model { get; set; } = new();
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    #endregion

    #region Provate Filled

    private FluentValidationValidator _fluentValidationValidator;
    private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
    private bool _processing;

    #endregion

    public void Cancel()
    {
        MudDialog.Cancel();
    }

    private async Task SaveAsync()
    {
        _processing = true;
        var response = await StaticPageManager.SaveAsync(Model);
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
}