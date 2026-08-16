using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;

namespace HiSubmit.Client.Infrastructure.Mappings;

public class VenueProfile:Profile
{
    public VenueProfile()
    {
        CreateMap<AddEditShowHallCommand, GetAllShowHallResponse>()
            .ReverseMap();
    }
}