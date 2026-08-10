using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Advertises.AddedAdvertise.Handlers;

public class SendNotificationAddedAdvertiseToAdminHandler : INotificationHandler<AddedAdvertiseEvent>
{
    private readonly INotificationService _notificationService;

    public SendNotificationAddedAdvertiseToAdminHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(AddedAdvertiseEvent notification, CancellationToken cancellationToken)
    {
        var notify = new AddAdminNotificationRequest
        {
            Link = "/admin/advertises",
            Title = "A new ad request has been registered",
            NotificationType = NotificationType.AdminAdvertiseRequest
        };
        await _notificationService.AddAdminNotificationJob(notify);
    }
}