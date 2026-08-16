using AutoMapper;
using HiSubmit.Application.Events.Users;
using HiSubmit.Application.Features.Festivals.Commands.AddEdiitEventOrginizer;
using HiSubmit.Application.Features.Festivals.Commands.AddEditAdditinalSettings;
using HiSubmit.Application.Features.Festivals.Commands.AddEditDeadLineEntry;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalContact;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalDeadlines;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalFile;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalVenue;
using HiSubmit.Application.Features.Festivals.Commands.CreateFestival;
using HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLine;
using HiSubmit.Application.Features.Festivals.Queries.GetAllDeadLineEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetAllFestivalFile;
using HiSubmit.Application.Features.Festivals.Queries.GetAllOrginizer;
using HiSubmit.Application.Features.Festivals.Queries.GetAllVenue;
using HiSubmit.Application.Features.Festivals.Queries.GetDeadLineById;
using HiSubmit.Application.Features.Festivals.Queries.GetDetailById;
using HiSubmit.Application.Features.Festivals.Queries.GetFestivalFileDetail;
using HiSubmit.Application.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Domain.Entities.Festivals;
using System.Linq;
using HiSubmit.Application.Features.Festivals.Commands.AddEditFestivalImages;
using HiSubmit.Application.Features.Festivals.Queries.GetAllImages;
using HiSubmit.Application.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;

namespace HiSubmit.Application.Mappings;

public partial class FestivalProfile : Profile
{
    public FestivalProfile()
    {
        CreateMap<AddEditFestivalDetailCommand, Festival>().ReverseMap();

        CreateMap<FestivalUserRegisteredEvent, Festival>()
            .ForMember(des => des.Name, map => map.MapFrom(src => src.FestivalName));
        CreateMap<Festival, GetFestivalDetailResponse>()
            .ForMember(p=>p.QualifyersId,map=>map.MapFrom(src=>src.FestivalFestivalQualifyings.Select(p=>p.FestivalQualifyingId)));

        CreateMap<GetAllEventOrganizerResponse, EventOrginizer>().ReverseMap();
        CreateMap<AddEditEventOrginizerCommand, EventOrginizer>().ReverseMap();
        CreateMap<AddEditFestivalDetailCommand, Festival>().ReverseMap();
        CreateMap<AddEditFestivalContactCommand, Festival>().ReverseMap();

        //Venue
        CreateMap<AddEditFestivalVenueCommand, Venue>().ReverseMap();
        CreateMap<GetAllVenueResponse,Venue>().ReverseMap()
            .ForMember(des=>des.ShowHallCount,map=>map.MapFrom(src=>src.ShowHalls.Count));
        CreateMap<Venue, GetVenueByIdResponse>().ReverseMap();

        //DeadLine
        CreateMap<AddEditFestivalDeadlineCommand, Festival>().ReverseMap();
        CreateMap<GetAllDeadLineResponse, DeadLine>().ReverseMap();
        CreateMap<GetDeadLineByIdResponse, DeadLine>().ReverseMap()
            .ForMember(des => des.CategoriesId, map => map
                .MapFrom(src => src.DeadlineEventCategories.Select(p => p.EventCategoryId)));

        CreateMap<AddEditDeadLineEntryCommand, DeadLine>().ReverseMap();
        CreateMap<AddEditAdditionalSettingCommand, Festival>().ReverseMap();

        CreateMap<UpdateFestivalArtCategory, FestivalArtCategory>().ReverseMap()
            .ForMember(p=>p.ArtCategoryName,map=>
                map.MapFrom(src=>src.ArtCategory.Name));
        CreateMap<UpdateFestivalFestivalFocus, FestivalFestivalFocus>()
            .ReverseMap()
            .ForMember(p=>p.FestivalFocusName,map=>
                map.MapFrom(src=>src.FestivalFocus.Name));

        CreateMap<DeadlineEventCategory, GetAllDeadLineEventCategoryResponse>()
            .ForMember(p => p.DeadLineDate, map => map.MapFrom(src => src.DeadLine.Date))
            .ForMember(p => p.CategoryName, map => map.MapFrom(src => src.EventCategory.Name))
            .ForMember(p=>p.EventCategory,map=>map.MapFrom(src=>src.EventCategory))
            .ReverseMap();


        //Festival File
        CreateMap<FestivalFile, GetFestivalFileDetailResponse>().ReverseMap();
        CreateMap<FestivalFile, GetAllFestivalFileResponse>().ReverseMap();
        CreateMap<FestivalFile, AddEditFestivalFileCommand>().ReverseMap();
            
            
        //festivalImage
        CreateMap<AddEditFestivalImageCommand, Image>().ReverseMap();
        CreateMap<FestivalImageDto, Image>().ReverseMap();
        CreateMap<GetAllFestivalImageResponse, Image>().ReverseMap();
            
        //festival names
        CreateMap<GetFestivalNamesResponse, Festival>().ReverseMap();
    }
}