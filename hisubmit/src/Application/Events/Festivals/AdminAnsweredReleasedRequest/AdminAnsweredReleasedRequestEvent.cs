using MediatR;

namespace HiSubmit.Application.Events.Festivals.AdminAnsweredReleasedRequest;

public class AdminAnsweredReleasedRequestEvent:INotification
{
    public int FestivalId { get; set; }
    public  bool IsEnable { get; set; }
}