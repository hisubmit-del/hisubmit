using HiSubmit.Application.Specifications.Base;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Enums;

namespace Hisubmit.Client.SharedModels.Validators.Features.Tickets;

public class GetAllSoldTicketFilter : HeroSpecification<SoldTicket>
{
    public GetAllSoldTicketFilter(int? festivalId, int? venueId, int? ticketId, string userId,string searchString,SoldTicketStatus? status)
    {
        Criteria = (soldTicket) =>
                (festivalId == null || soldTicket.Ticket.Venue.FestivalId == festivalId) &&
                (venueId == null || soldTicket.Ticket.VenueId == venueId) &&
                (ticketId == null || soldTicket.TicketId == ticketId) &&
                (string.IsNullOrWhiteSpace(userId) || soldTicket.UserId == userId)&&
                (status==null || soldTicket.SoldTicketStatus==status)&&
                (string.IsNullOrWhiteSpace(searchString)|| soldTicket.OtherUserEmail.Contains(searchString))
            ;
    }
}