using MediatR;

namespace HiSubmit.Application.Events.TicketsSold;

public class PaidTicketEvent:INotification
{
    public  int SoldTicketId { get; set; }
}