using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Client.Infrastructure.Managers.PublicFestival;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HiSubmit.Web.Components.Pages.Public.Festivals.Components
{
    public partial class FestivalVenue
    {
        [Parameter]
        public int FestivalId { get; set; }

        [Inject]
        public IPublicFestivalManager FestivalManager { get; set; }

        private List<GetAllVenueResponse> Venues { get; set; }

        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            await LoadOrganizer();
            await base.OnInitializedAsync();
            _loaded = true;
        }
        private async Task LoadOrganizer()
        {
            var response = await FestivalManager.GetAllVenueAsync(new GetAllVenueQuery()
            {
                FestivalId = FestivalId,
            });
            if (response.Succeeded)
            {
                Venues = response.Data;
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
