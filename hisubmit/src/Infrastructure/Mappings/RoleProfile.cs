using AutoMapper;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Infrastructure.Models.Identity;

namespace HiSubmit.Infrastructure.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<RoleResponse, BlazorHeroRole>().ReverseMap();
        }
    }
}