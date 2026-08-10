using MediatR;

namespace HiSubmit.Application.Events.Users;

public class FestivalUserRegisteredEvent:INotification
{
    public string UserId { get; set; }
    public string FestivalName { get; set; }
}