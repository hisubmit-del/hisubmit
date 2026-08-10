using AutoMapper;
using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Infrastructure.Models.Identity;

namespace HiSubmit.Infrastructure.Mappings
{
    public class ChatHistoryProfile : Profile
    {
        public ChatHistoryProfile()
        {
            CreateMap<ChatHistory<IChatUser>, ChatHistory<BlazorHeroUser>>().ReverseMap();
        }
    }
}