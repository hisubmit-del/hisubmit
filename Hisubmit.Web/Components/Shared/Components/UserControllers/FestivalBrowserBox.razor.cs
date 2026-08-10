using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.Pages.Public.Festivals.Components;
using HiSubmit.Web.Components.Shared.Dialogs;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HiSubmit.Web.Components.Shared.Components.UserControllers;

public partial class FestivalBrowserBox
{
    [Parameter] public GetAllFestivalResponse Festival { get; set; } = new();
    private async Task Submit()
    {
        var festivalId = Festival.Id;
        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            
        };
        var user = await AuthenticationManager.CurrentUser();
        if (!user.Identity.IsAuthenticated)
        {
            var option = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                
            };
            var parameters = new DialogParameters();
            _dialogService.Show<NeedToLogin>("Need To Login", parameters, option);
        }
        else
        {
            var parameter = new DialogParameters
            {
                { nameof(FestivalCategorySelected.FestivalId), festivalId }
            };
            _dialogService.Show<FestivalCategorySelected>(localizer["Selected category"], parameter, options);
        }
    }

    private void GoToFestivalDetail()
    {
        _navigationManager.NavigateTo($"/festivalPage/{Festival.URL}");
    }
}