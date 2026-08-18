using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;

namespace Web.Components.Shared.Components;

public partial class FavoriteFestival
{
    private PaginatedResult<GetAllFestivalResponse> _festivalResponse=new(new());
    [Inject] private IPublicFestivalManager FestivalManager { get; set; }

    [Parameter]
    public string Class { get; set; }
  
    protected override async Task OnInitializedAsync()
    {
        await LoadFestivals();
        
        await base.OnInitializedAsync();
    }

    private async Task LoadFestivals()
    {
        var response =
            await FestivalManager.GetAllFestival(new GetAllFestivalRequest()
            {
                PageSize = 8
            });
        if (response.Succeeded)
        {
            _festivalResponse = response;
        }
        else
        {
            foreach (var message in response.Messages)
            {
                _snackBar.Add(message, MudBlazor.Severity.Error);
            }
        }
    }
}
