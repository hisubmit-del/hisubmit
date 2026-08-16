using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.Handler;

public class SendNotificationNewFestivalToAdmin : INotificationHandler<CreatedFestival>
{
    private readonly INotificationService _notificationService;

    public SendNotificationNewFestivalToAdmin(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(CreatedFestival notification, CancellationToken cancellationToken)
    {
        var notif = new AddAdminNotificationRequest
        {
            Link = "admin/festivals",
            NotificationType = NotificationType.AdminNewFestival,
            Title = "The new festival was registered on the site",
        };
        await _notificationService.AddAdminNotificationJob(notif);
    }
}