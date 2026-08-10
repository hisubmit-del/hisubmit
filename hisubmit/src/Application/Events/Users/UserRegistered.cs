using MediatR;

namespace HiSubmit.Application.Events.Users;

public class UserRegisteredEvent:INotification
{
    public string FullName { get; set; }
    public  string Email { get; set; }
    public  string UserId { get; set; }

    public string VerificationUrl { get; set; }
}