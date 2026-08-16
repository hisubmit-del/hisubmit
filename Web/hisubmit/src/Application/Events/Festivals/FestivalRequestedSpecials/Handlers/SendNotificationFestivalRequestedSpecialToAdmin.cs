using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.FestivalREquestedSpecials;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Festivals.FestivalRequestedSpecials.Handlers;


public class SendNotificationFestivalRequestedSpecialToAdmin : INotificationHandler<FestivalRequestedSpecial>
{
    private readonly INotificationService _notificationService;

    public SendNotificationFestivalRequestedSpecialToAdmin(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }
    public async Task Handle(FestivalRequestedSpecial notification, CancellationToken cancellationToken)
    {
        var notify = new AddAdminNotificationRequest
        {
            Link = "admin/festivals/",
            Title = "A festival requests Gold",
            NotificationType = NotificationType.AdminSpecialFestivalRequest
        };
        await _notificationService.AddAdminNotificationJob(notify);
    }
}
