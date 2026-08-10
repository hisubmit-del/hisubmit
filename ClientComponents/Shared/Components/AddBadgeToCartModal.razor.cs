using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using HiSubmit.Client.Infrastructure.Managers.SoldTickets;
using HiSubmit.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Shared.Components;

public partial class AddBadgeToCartModal
{
    [Inject] private ISoldTicketManager SoldTicketManager { get; set; }
    [Inject] private  UserCartService UserCartService { get; set; }

    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; }  
    
    [Parameter]
    public AddSoldBadgeCommand SoldBadge { get; set; }
    
    [Parameter]
    public  int MaxCount { get; set; }

    private FluentValidationValidator _fluentValidationValidator;
    private bool _processing;

    private async Task SaveAsync()
    {
        var validated = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (validated)
        {
            _processing = true;
            var response = await SoldTicketManager.AddBadgeToCart(SoldBadge);
            if (response.Succeeded)
            {
                _snackBar.Add(response.Messages[0], Severity.Success);
                UserCartService.ChangeUserCart();
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

    private void Cancel()
    {
        MudDialog.Close();
    }
}