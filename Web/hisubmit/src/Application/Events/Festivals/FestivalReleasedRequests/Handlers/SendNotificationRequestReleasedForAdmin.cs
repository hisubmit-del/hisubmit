using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.FestivalReleasedRequests.Handlers;

public class SendNotificationRequestReleasedForAdmin : INotificationHandler<FestivalRequestedReleased>
{
    private readonly INotificationService _notificationService;

    public SendNotificationRequestReleasedForAdmin(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(FestivalRequestedReleased notification, CancellationToken cancellationToken)
    {
        var notify = new AddAdminNotificationRequest
        {
            Link = "admin/festivals/",
            Title = "A festival requests publication",
            NotificationType = NotificationType.AdminReleaseFestivalRequest
        };
        await _notificationService.AddAdminNotificationJob(notify);
    }
}
