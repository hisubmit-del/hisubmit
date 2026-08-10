using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Tickets.AddTickets.Handlers;

public class SendNotificationAddedTicketsHandler(INotificationService notificationService)
    : INotificationHandler<AddedTicketEvent>
{
    public async Task Handle(AddedTicketEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.AddAdminNotificationJob(new AddAdminNotificationRequest()
        {
            Link = $"admin/festival/tickets/{notification.FestivalId}",
            Title = $"{notification.FestivalName} Festival has defined a new ticket",
            NotificationType = NotificationType.AdminNewAddedTicketOrBadge
        });
    }
}