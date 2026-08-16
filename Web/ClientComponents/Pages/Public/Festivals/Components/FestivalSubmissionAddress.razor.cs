using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;
using ClientComponents.Shared.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ClientComponents.Pages.Public.Festivals.Components
{
    public partial class FestivalSubmissionAddress
    {
        [Parameter]
        public GetAllVenueResponse Venue { get; set; }

        public async Task TakeTicket()
        {
            var dialogOption = new DialogOptions()
            {
                CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true
            };
            // var parameters = new DialogParameters
            // { { nameof(AddBadgeToCartModal.SoldBadge), new AddSoldBadgeCommand()
            // {
            //     TicketId = 
            // } } };
            // parameters.Add(nameof(AddBadgeToCartModal.MaxCount));

          //  var result = _dialogService.Show<AddBadgeToCartModal>("Add To Cart", parameters, dialogOption);
        }
    }
}
