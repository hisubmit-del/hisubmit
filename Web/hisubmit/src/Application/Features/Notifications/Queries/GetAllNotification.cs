using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Requests;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;

namespace HiSubmit.Application.Features.Notifications.Queries;

public class GetAllNotificationQuery : PagedRequest, IRequest<PaginatedResult<GetAllNotificationResponse>>
{
    public bool? Seen { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public SiteAccountType SiteAccountType { get; set; }
}

public class
    GetAllNotificationQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,ICurrentUserService currentUserService)
    : IRequestHandler<GetAllNotificationQuery,
        PaginatedResult<GetAllNotificationResponse>>
{
    public async Task<PaginatedResult<GetAllNotificationResponse>> Handle(GetAllNotificationQuery request,
        CancellationToken cancellationToken)
    {
        PaginatedResult<GetAllNotificationResponse> notifications;

        switch (request.SiteAccountType)
        {
            case SiteAccountType.User:
                request.UserId = currentUserService.UserId;
                notifications = await unitOfWork.Repository<Notification>()
                    .Entities
                    .Where(p => p.UserId == request.UserId
                                &&(request.Seen == null || p.Seen == request.Seen!.Value)
                                && p.SiteAccountType == SiteAccountType.User)
                    .ProjectTo<GetAllNotificationResponse>(mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(request);
                break;
            case SiteAccountType.Admin:
                notifications = await unitOfWork.Repository<Notification>()
                    .Entities
                    .Where(p =>   (request.Seen == null || p.Seen == request.Seen!.Value)
                                  && p.SiteAccountType == SiteAccountType.Admin)
                    .ProjectTo<GetAllNotificationResponse>(mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(request);
                break;
            case SiteAccountType.Festival:
                notifications = await unitOfWork.Repository<Notification>()
                    .Entities
                    .Where(p => p.FestivalId == request.FestivalId
                                   &&(request.Seen == null || p.Seen == request.Seen!.Value)
                                     && p.SiteAccountType == SiteAccountType.Festival)
                    .ProjectTo<GetAllNotificationResponse>(mapper.ConfigurationProvider)
                    .ToPaginatedListAsync(request);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return notifications;
    }
}

public class GetAllNotificationResponse
{
    public int Id { get; set; }
    public bool Seen { get; set; }
    public string Link { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public DateTime CreatedOn { get; set; }
    public SiteAccountType SiteAccountType { get; set; }
    public NotificationType NotificationType { get; set; }
}