using AutoMapper;
using HiSubmit.Application.Features.Chats.Commands;
using HiSubmit.Application.Features.Chats.Queries;
using Hisubmit.Client.SharedModels.Enums.Chats;
using HiSubmit.Domain.Entities.Chats;

namespace HiSubmit.Application.Mappings;

public class ChatProfile:Profile
{
    public ChatProfile()
    {
        CreateMap<TryGetRoomIdCommand, ChatRoom>().ReverseMap();
        CreateMap<AddChatMessageCommand, ChatMessage>()
           .ReverseMap()
           .ForMember(p=>p.Type,
               map=>map.MapFrom(d=>d.Type));

        CreateMap<ChatMessageType, Domain.Enums.Chats.ChatMessageType>();
        CreateMap<GetChatHistoryResponse, ChatMessage>().ReverseMap();
    }
}