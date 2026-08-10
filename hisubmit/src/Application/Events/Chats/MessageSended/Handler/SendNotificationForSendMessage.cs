using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Services;
using Hisubmit.Client.SharedModels.Enums.Chats;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Chats.MessageSended.Handler;

public class SendNotificationForSendMessage(INotificationService notificationService)
    : INotificationHandler<MessageSendedEvent>
{
    public async Task Handle(MessageSendedEvent notification, CancellationToken cancellationToken)
    {
        switch (notification.ChatMessageType)
        {
            case ChatMessageType.UserToUser:
                break;
            case ChatMessageType.UserToAdmin:
                var notify = new AddAdminNotificationRequest
                {
                    Link = "admin/chat",
                    NotificationType = NotificationType.AdminReceivedMessage,
                    Title = "You have a new Message"
                };
                await notificationService.AddAdminNotificationJob(notify);
                break;
            case ChatMessageType.UserToFestival:
                var notify2 = new AddFestivalNotificationRequest
                { 
                    Link = "festival/chat",
                    NotificationType = NotificationType.FestivalReceivedMessage,
                    Title = "You have a new Message",
                    FestivalId = notification.ChatRoom.FestivalId!.Value
                };
                await notificationService.AddFestivalNotificationJob(notify2);
                break;
            case ChatMessageType.FestivalToUser:
                var notify3 = new AddUserNotificationRequest()
                {
                    Link = "chat",
                    NotificationType = NotificationType.UserReceivedMessage,
                    Title = "You have a new Message",
                    UserId = notification.ChatRoom.ChatUser1
                };
                await notificationService.AddUserNotificationJob(notify3);
                break;
            case ChatMessageType.FestivalToAdmin:
                var notify4 = new AddUserNotificationRequest()
                {
                    Link = "admin/chat",
                    NotificationType = NotificationType.AdminReceivedMessage,
                    Title = "You have a new Message",
                };
                await notificationService.AddUserNotificationJob(notify4);
                break;
            case ChatMessageType.FestivalToFestival:
                break;
            case ChatMessageType.AdminToUser:
                var notify5 = new AddUserNotificationRequest()
                {
                    Link = "user/chat",
                    NotificationType = NotificationType.UserReceivedMessage,
                    Title = "You have a new Message",
                    UserId = notification.ChatRoom.ChatUser1
                };
                await notificationService.AddUserNotificationJob(notify5);
                break;
            case ChatMessageType.AdminToFestival:
                var notify6 = new AddFestivalNotificationRequest()
                {
                    Link = "festival/chat",
                    NotificationType = NotificationType.FestivalReceivedMessage,
                    Title = "You have a new Message",
                    FestivalId = notification.ChatRoom.FestivalId!.Value
                };
                await notificationService.AddFestivalNotificationJob(notify6);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}