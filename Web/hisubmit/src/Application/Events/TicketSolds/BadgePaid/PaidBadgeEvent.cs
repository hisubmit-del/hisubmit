using MediatR;

namespace HiSubmit.Application.Events.TicketsSold;

public class PaidBadgeEvent:INotification
{
    public  int SoldTicketId { get; set; }
}