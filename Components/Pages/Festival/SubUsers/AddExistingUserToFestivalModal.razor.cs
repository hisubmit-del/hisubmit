using System.Threading.Tasks;
using Blazored.FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;
using Hisubmit.Client.SharedModels.Features.SubUsers.Commands.AddExistingUserToFestival;
using HiSubmit.Client.Infrastructure.Managers.FestivalSubUsers;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Web.Components.Pages.Festival.SubUsers;

public partial class AddExistingUserToFestivalModal
{
    [Inject] private  IFestivalSubUserManager FestivalSubUserManager { get; set; }

    [Parameter]
    public  int FestivalId { get; set; }
    [CascadingParameter]public IMudDialogInstance MudDialog { get; set; }
    
    private AddExistingUserToFestivalCommand _festival=new ();

    private FluentValidationValidator _fluentValidationValidator;
    private bool _validate = true;

    private bool _processing;
    private async Task SubmitFestival()
    {
        _validate = _fluentValidationValidator.Validate(param => param.IncludeAllRuleSets());
        if (_validate)
        {
            _processing = true;
            _festival.FestivalId = FestivalId;
            var response = await FestivalSubUserManager.AddExistingUserToFestival(_festival);
            if (response.Succeeded)
            {
                MudDialog.Close();
              
                _snackBar.Add(response.Messages[0], Severity.Success);
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