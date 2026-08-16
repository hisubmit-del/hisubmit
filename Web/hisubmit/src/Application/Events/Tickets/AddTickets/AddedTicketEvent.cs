using MediatR;

namespace HiSubmit.Application.Events.Tickets.AddTickets;

public class AddedTicketEvent:INotification
{
    public int  TicketId { get; set; }
    public int FestivalId { get; set; }
    public string FestivalName { get; set; }
}