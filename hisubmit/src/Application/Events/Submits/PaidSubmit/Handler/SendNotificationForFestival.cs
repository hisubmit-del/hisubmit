using MediatR;
using System.Linq;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Application.Interfaces.Repositories;

namespace HiSubmit.Application.Events.Submits.PaidSubmit.Handler;

public class SendNotificationForFestival:INotificationHandler<PaidSubmitEvent>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly INotificationService _notificationService;

    public SendNotificationForFestival
        (IUnitOfWork<int> unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }
    public async Task Handle(PaidSubmitEvent notification, CancellationToken cancellationToken)
    {
        var festivalId = await _unitOfWork.Repository<Submit>()
            .Entities
            .Where(p => p.Id == notification.SubmitId)
            .Select(p => p.FestivalId)
            .FirstOrDefaultAsync(cancellationToken);
        
        var notif = new AddFestivalNotificationRequest
        {
            FestivalId = festivalId,
            Link = "festival/submits",
            NotificationType = NotificationType.FestivalNewSubmit,
            Title = "The new project has been sent to your festival",
        };
        await _notificationService.AddFestivalNotificationJob(notif);
    }
}