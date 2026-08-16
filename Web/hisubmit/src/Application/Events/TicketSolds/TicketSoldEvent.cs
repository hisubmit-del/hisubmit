using HiSubmit.Domain.Entities.Festivals.Tickets;
using MediatR;

namespace HiSubmit.Application.Events.TicketsSold;

public record TicketSoldEvent:INotification
{
   public  int TicketSoldId { get; set; }
}
