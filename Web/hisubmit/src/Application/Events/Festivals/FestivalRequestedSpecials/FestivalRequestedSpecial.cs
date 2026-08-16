using MediatR;

namespace HiSubmit.Application.Events.Festivals.FestivalREquestedSpecials;

public class FestivalRequestedSpecial:INotification
{
    public  int FestivalId { get; set; }
}