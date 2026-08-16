using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.RefeerrAddToProjects.Handlers;

public class SendNotificationForReferee(INotificationService notificationService)
    : INotificationHandler<RefereeAddToProjectsEvent>
{
    public async Task Handle(RefereeAddToProjectsEvent notification, CancellationToken cancellationToken)
    {
        await notificationService.AddUserNotificationJob(new AddUserNotificationRequest
        {
            Link = "referees",
            UserId = notification.UserId,
            Title = "You have been selected to judge the project",
            NotificationType = NotificationType.RefereeAddToProject,
        });
    }
}