using MediatR;

namespace HiSubmit.Application.Events.FestivalRegisteredUser;

public class FestivalRegisteredUserEvent : INotification
{

    public string Email { get; set; }
    public string Password { get; set; }
    public  string FullName { get; set; }
    public int FestivalId { get; set; }
}
