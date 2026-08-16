using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Tickets.Commands.Enable;

public class EnableTicketCommand
{
    public int TicketId { get; set; }
    public bool IsEnable { get; set; }

   public ShowInSiteStatus Status { get; set; }

}
