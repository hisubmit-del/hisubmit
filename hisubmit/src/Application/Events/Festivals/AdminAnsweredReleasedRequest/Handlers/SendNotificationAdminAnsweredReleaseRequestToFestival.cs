using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.AdminAnsweredReleasedRequest.Handlers;

public class SendNotificationAdminAnsweredReleaseRequestToFestival
    : INotificationHandler<AdminAnsweredReleasedRequestEvent>
{
    private readonly INotificationService _notificationService;

    public SendNotificationAdminAnsweredReleaseRequestToFestival(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(AdminAnsweredReleasedRequestEvent notification, CancellationToken cancellationToken)
    {
        var notify=new AddFestivalNotificationRequest
        {
          Link  = "festival/edit",
          FestivalId = notification.FestivalId,
          Title = "Your festival publication request has been answered",
          NotificationType = NotificationType.FestivalAnsweredReleasedRequest,
        };
        await _notificationService.AddFestivalNotificationJob(notify);
    }
}