namespace Hisubmit.Client.SharedModels.Features.SoldTickets.Commands;

public class AddSoldBadgeCommand
{
    public  string OtherUserEmail { get; set; }
    public  bool ForOtherUser { get; set; }
    public  int TicketId { get; set; }
    public  int Count { get; set; }
}