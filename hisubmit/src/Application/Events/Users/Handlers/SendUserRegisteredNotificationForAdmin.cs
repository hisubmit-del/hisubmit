using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Users.Handlers;

public class SendUserRegisteredNotificationForAdmin(INotificationService notificationService)
    : INotificationHandler<UserRegisteredEvent>
{
    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var notify = new AddAdminNotificationRequest
        {
            Link = "identity/users",
            NotificationType = NotificationType.AdminNewRegister,
            Title = "New user registered on the site"
        };

       await notificationService.AddAdminNotificationJob(notify);
    }
}