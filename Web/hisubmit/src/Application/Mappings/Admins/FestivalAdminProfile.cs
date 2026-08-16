using AutoMapper;
using HiSubmit.Application.Features.AdminFestival.Queries.GetAllFestival;
using HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalFile;
using HiSubmit.Domain.Entities.Festivals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Domain.Entities.Locations;

namespace HiSubmit.Application.Mappings.Admins;

public class FestivalAdminProfile : Profile
{
    public FestivalAdminProfile()
    {
        CreateMap<GetAllFestivalResponse, Festival>().ReverseMap()
            .ForMember(p => p.UserId, map => map.MapFrom(src => src.UserId))
            .ForMember(p => p.Focuses, map =>
                map.MapFrom(src => src.FestivalFestivalFoci.Select(p => p.FestivalFocus.Name).ToList()))
            // .ForMember(p => p.SelectedQualifiersId,
            //     map => map.MapFrom(src => src.FestivalFestivalQualifyings!.Select(p => p.FestivalQualifyingId)))
            ;

        CreateMap<AddEditAddressCommand, Address>()
            .ReverseMap()
            .ForMember(p=>p.CountryName,map=>map.MapFrom(src=>TakeCountryNameIfExist(src)));

        CreateMap<AddEditDeadLineEntryRequest, DeadLine>()
            .ReverseMap();
    }

    private static string TakeCountryNameIfExist(Address address)
    {
        string countryName = string.Empty;
        if(address.Country != null)
        {
            countryName = address.Country.Name;
        }
        return countryName;
    }
        
    }
