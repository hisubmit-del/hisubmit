using System.Linq;
using AutoMapper;
using HiSubmit.Application.Features.Festivals.Commands.AddEditDeadLineEntry;
using HiSubmit.Application.Features.Festivals.Commands.AddEditEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetAllEventCategory;
using HiSubmit.Application.Features.Festivals.Queries.GetEventCateoryById;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Application.Mappings
{
    public partial class FestivalProfile
    {
        public class EventCategoryProfile : Profile
        {
            public EventCategoryProfile()
            {
                CreateMap<GetAllEventCategoryResponse, EventCategory>().ReverseMap();
                CreateMap<GetEventCategoryByIdResponse , EventCategory>().ReverseMap()
                    .ForMember(des=>des.DeadLineCategories,map=>map.MapFrom(src=>src.DeadlineEventCategories))
                    .ForMember(des=>des.CountriesId,map=>map.MapFrom(src=>src.EventCategoryCountries.Select(p=>p.CountryId)))
                    .ForMember(des=>des.CountriesName,map=>map.MapFrom(src=>src.EventCategoryCountries.Select(p=>p.Country.Name)));

                CreateMap<DeadlineEventCategory, AddEditDeadLineEntryCommand>().ReverseMap();
                CreateMap<AddEditEventCategoryCommand,EventCategory>().ReverseMap();
                CreateMap<UpdateDeadlineCategoryonFee, DeadlineEventCategory>().ReverseMap()
                    .ForMember(p=>p.DeadLineName,map=>map.MapFrom(des=>des.DeadLine.Name));
            }
        }
    }
}
