using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.ViolationReportField.Handlers;

public class SendNotificationViolationReportFieldToAdminHandlers : INotificationHandler<ViolationReportFieldEvent>
{
    private readonly INotificationService _notificationService;

    public SendNotificationViolationReportFieldToAdminHandlers(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(ViolationReportFieldEvent notification, CancellationToken cancellationToken)
    {
        var notify = new AddAdminNotificationRequest
        {
            Link = "admin/reviews",
            Title = "A festival violation report was registered",
            NotificationType = NotificationType.AdminReportViolationFestival
        };
        await _notificationService.AddAdminNotificationJob(notify);
    }
}
