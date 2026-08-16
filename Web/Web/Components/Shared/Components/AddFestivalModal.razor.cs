using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;
using HiSubmit.Client.Infrastructure.Managers.Festivals;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Shared.Components;

public partial class AddFestivalModal
{
    [Inject]private  IFestivalManager FestivalManager { get; set; }
    [CascadingParameter]public IMudDialogInstance MudDialog { get; set; }
    
    private AddFestivalCommand _festival=new ();

    private FluentValidationValidator _fluentValidationValidator;
    private bool _validate = true;
    private bool _processing;
    private async Task SubmitFestival()
    {
        _processing = true;
        _validate = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (_validate)
        {
            var response = await FestivalManager.AddFestival(_festival);
            if (response.Succeeded)
            {
                MudDialog.Close();
             //   await   AuthenticationManager.TryForceRefreshToken();
                _navigationManager.NavigateTo(_navigationManager.Uri,true);
                _snackBar.Add(response.Messages[0], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        _processing = false;
    }

    private void Cancel()
    {
        MudDialog.Close();
    }
}