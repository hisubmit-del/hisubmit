using MediatR;

namespace HiSubmit.Application.Events.TicketsSold;

public class BadgeSoldEvent:INotification
{
    public  int TicketSoldId { get; set; }
}