using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.AdminAnsweredSpecialRequest.Handlers;

public class SendNotificationAdminAnsweredSpecialRequestToFestival:
    INotificationHandler<AdminAnsweredSpecialRequestEvent>
{
    private readonly INotificationService _notificationService;

    public SendNotificationAdminAnsweredSpecialRequestToFestival(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(AdminAnsweredSpecialRequestEvent notification, CancellationToken cancellationToken)
    {
        var notify=new AddFestivalNotificationRequest
        {
            Link  = "festival/edit",
            FestivalId = notification.FestivalId,
            Title = "Your festival special request has been answered",
            NotificationType = NotificationType.FestivalAnsweredReleasedRequest,
        };
        await _notificationService.AddFestivalNotificationJob(notify);
    }
}
