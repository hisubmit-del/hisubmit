using AutoMapper;
using HiSubmit.Application.Features.Advertises.Commands;
using HiSubmit.Application.Features.Advertises.Queries;
using Hisubmit.Client.SharedModels.Features.Advertises.Commands;
using Hisubmit.Client.SharedModels.Features.Advertises.Queries;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Files;

namespace HiSubmit.Application.Mappings;

public class AdvertiseProfile:Profile
{
    public AdvertiseProfile()
    {
        CreateMap<ImageDto, Image>().ReverseMap();
        CreateMap<AttachFileDto, AttachFile>().ReverseMap();
        CreateMap<AddAdvertiseCommand,AdvertiseRequest>().ReverseMap();
        CreateMap<GetAllAdvertiseResponse, AdvertiseRequest>().ReverseMap();
        CreateMap<GetDetailAdvertiseResponse, AdvertiseRequest>()
            .ReverseMap()
            .ForMember(des => des.Images, map => map.MapFrom(src => src.Images))
            .ForMember(des => des.Files, map => map.MapFrom(src => src.Files))
            ;
        CreateMap<GetAllAdvertiseBannerResponse, AdvertiseBanner>().ReverseMap();
        CreateMap<AddEditAdvertiseBannerCommand, AdvertiseBanner>().ReverseMap();
    }
}