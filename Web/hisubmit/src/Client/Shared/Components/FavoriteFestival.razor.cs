using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.AspNetCore.Components;

namespace HiSubmit.Client.Shared.Components;

public partial class FavoriteFestival
{
    private PaginatedResult<GetAllFestivalResponse> _festivalResponse=new(new());
    [Inject] private IPublicFestivalManager FestivalManager { get; set; }

    [Parameter]
    public string Class { get; set; }
  
    private PersistingComponentStateSubscription _subscription;

    private Task PersistFestival()
    {
        ApplicationState.PersistAsJson("favoriteFestivals", _festivalResponse);
        return Task.CompletedTask;
    }
    protected override async Task OnInitializedAsync()
    {
        ApplicationState.RegisterOnPersisting(PersistFestival);
        await LoadFestivals();
        
        await base.OnInitializedAsync();
    }

    private async Task LoadFestivals()
    {
        if (ApplicationState.TryTakeFromJson
                <PaginatedResult<GetAllFestivalResponse>>
                ("favoriteFestivals", out var stored))
        {
            _festivalResponse = stored;
        }
        else
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
}