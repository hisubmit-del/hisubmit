using AutoMapper;
using HiSubmit.Application.Features.Notifications.Queries;
using HiSubmit.Application.Services;
using HiSubmit.Domain.Entities;

namespace HiSubmit.Application.Mappings;

public class NotificationProfile : Profile
{
    public NotificationProfile()
    {
        CreateMap<GetAllNotificationResponse, Notification>()
            .ReverseMap();

        CreateMap<AddUserNotificationRequest, Notification>()
            .ReverseMap();

        CreateMap<AddFestivalNotificationRequest, Notification>()
            .ReverseMap();

        CreateMap<AddAdminNotificationRequest, Notification>()
            .ReverseMap();
    }
}