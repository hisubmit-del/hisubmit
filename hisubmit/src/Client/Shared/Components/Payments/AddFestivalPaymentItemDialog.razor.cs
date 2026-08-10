using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Commands.Add;
using HiSubmit.Client.Infrastructure.Managers.AdminPaymentManager;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Client.Shared.Components.Payments;

public partial class AddFestivalPaymentItemDialog
{
    #region Inject

    [Inject] private IAdminPaymentManager PaymentManager { get; set; }

    #endregion

    #region Parameters

    [Parameter] public AddFestivalPaymentItemCommand Item { get; set; }
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }

    #endregion

    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    private async Task SaveAsync()
    {
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            _processing = true;
            var response = await PaymentManager.AddFestivalPaymentItem(Item);
            if (response.Succeeded)
            {
                MudDialog.Close();
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                _snackBar.Add(response.Messages[0], Severity.Error);
            }
            _processing = false;
        }
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}