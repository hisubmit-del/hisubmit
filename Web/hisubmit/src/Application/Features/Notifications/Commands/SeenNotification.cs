using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Notifications.Commands;

public class SeenNotificationCommand : IRequest<IResult>
{
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public SiteAccountType AccountType { get; set; }
    public NotificationType NotificationTypes { get; set; }
}

internal class SeenNotificationCommandHandler : IRequestHandler<SeenNotificationCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public SeenNotificationCommandHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(SeenNotificationCommand request, CancellationToken cancellationToken)
    {
        List<Notification> notifications;
        switch (request.AccountType)
        {
            case SiteAccountType.User:
                notifications = await _unitOfWork.Repository<Notification>()
                    .Entities.Where(p => p.UserId == request.UserId &&
                                         p.NotificationType == request.NotificationTypes
                                         && !p.Seen).ToListAsync(cancellationToken);
                break;
            case SiteAccountType.Admin:
                notifications = await _unitOfWork.Repository<Notification>()
                    .Entities.Where(p => p.SiteAccountType == SiteAccountType.Admin
                                         && p.NotificationType == request.NotificationTypes
                                         && !p.Seen).ToListAsync(cancellationToken);
                break;
            case SiteAccountType.Festival:
                notifications = await _unitOfWork.Repository<Notification>()
                    .Entities.Where(p => p.SiteAccountType == SiteAccountType.Festival
                                         && p.FestivalId == request.FestivalId
                                         && p.NotificationType == request.NotificationTypes
                                         && !p.Seen).ToListAsync(cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        foreach (var notification in notifications)
        {
            notification.Seen = true;
            await _unitOfWork.Repository<Notification>()
                .UpdateAsync(notification);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("notification Seen");
    }
}