using MediatR;

namespace HiSubmit.Application.Events.Festivals.FestivalReleasedRequests;

public class FestivalRequestedReleased:INotification
{
    public  int FestivalId { get; set; }
}