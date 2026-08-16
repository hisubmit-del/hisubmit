using AutoMapper;
using HiSubmit.Domain.Entities.Festivals;
using Hisubmit.Hisubmit.Client.SharedModels.Features.MasterFestivals.Queries;

namespace HiSubmit.Application.Mappings;

public class FestivalMasterProfile:Profile
{
    public FestivalMasterProfile()
    {
        CreateMap<GetAllMasterFestivalResponse, FestivalMaster>()
            .ReverseMap();
    }
}