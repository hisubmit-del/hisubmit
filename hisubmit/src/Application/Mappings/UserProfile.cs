using AutoMapper;
using HiSubmit.Application.Features.Users.Commands.Register;
using HiSubmit.Application.Requests.Identity;

namespace HiSubmit.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterUserCommand, RegisterRequest>().ReverseMap();
        }
    }

}