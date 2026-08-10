using Hisubmit.Client.SharedModels.Enums.Chats;
using HiSubmit.Domain.Entities.Chats;

using MediatR;

namespace HiSubmit.Application.Events.Chats.MessageSended;

public class MessageSendedEvent:INotification
{
    public ChatRoom  ChatRoom { get; set; }
    public ChatMessageType ChatMessageType { get; set; }
}