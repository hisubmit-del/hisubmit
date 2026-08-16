namespace Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;

public record AddSoldTicketCommand
{
    public int Count { get; set; }
    public  int VenueId { get; set; }
    public int TicketId { get; set; }
    public int? ShowTimeId { get; set; }
    public int? ChairNumber { get; set; }
    public  bool ForOtherUser { get; set; }
    public  string OtherUserEmail { get; set; }
}