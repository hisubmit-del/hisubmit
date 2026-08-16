using AutoMapper;
using HiSubmit.Application.Features.Festivals.Commands.AddEditShowHall;
using HiSubmit.Application.Features.Festivals.Queries.GetAllShowHall;
using HiSubmit.Application.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Mappings;

public class ShowHallProfile:Profile
{
    public ShowHallProfile()
    {
        CreateMap<AddEditShowHallCommand, ShowHall>()
            .ForMember(p => p.ShowTimes, map => map.Ignore());
        CreateMap<GetAllShowHallResponse, ShowHall>().ReverseMap();
        CreateMap<ShowHall, ShowHallDto > ().ReverseMap();
        CreateMap<ShowTime, ShowTimeDto>().ReverseMap();
    }
}
