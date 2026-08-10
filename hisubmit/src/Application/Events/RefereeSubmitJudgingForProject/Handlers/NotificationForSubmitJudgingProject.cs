using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.RefereeSubmitJudgingForProject.Handlers;

public class NotificationForSubmitJudgingProject
    : INotificationHandler<RefereeSubmitJudgingFroProjectEvent>
{
    private readonly INotificationService _notificationService;

    public NotificationForSubmitJudgingProject(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task Handle(RefereeSubmitJudgingFroProjectEvent notification,
        CancellationToken cancellationToken)
    {
       await _notificationService.AddFestivalNotificationJob(new AddFestivalNotificationRequest()
        {
            FestivalId = notification.FestivalId,
            Link = $"festival/judgingResult/{notification.SubmitId}/",
            NotificationType = NotificationType.FestivalRefereeSubmitJudgingResult,
            Title = "A new arbitration has been registered"
        });
    }
}
