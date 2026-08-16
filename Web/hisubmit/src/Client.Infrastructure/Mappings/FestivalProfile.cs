using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalFile;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetEventCateoryById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalFileDetail;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;

namespace HiSubmit.Client.Infrastructure.Mappings
{
    public class FestivalProfile:Profile
    {
        public FestivalProfile()
        {
            CreateMap<AddEditFestivalDetailCommand, GetFestivalDetailResponse>().ReverseMap();
            CreateMap<AddEditFestivalContactCommand,GetFestivalDetailResponse>().ReverseMap();
            CreateMap<AddEditFestivalVenueCommand, GetVenueByIdResponse>().ReverseMap();
            CreateMap<AddEditFestivalDeadlineCommand, GetFestivalDetailResponse>().ReverseMap();
            CreateMap<AddEditDeadLineEntryRequest, GetDeadLineByIdResponse>().ReverseMap()
                .ForMember(des=>des.CategoryId,map=>map.MapFrom(src=>src.CategoriesId));


            CreateMap<AddEditEventCategoryCommand, GetEventCategoryByIdResponse>().ReverseMap()
                .ForMember(p => p.CategoryonFees, map => map.MapFrom(src => src.DeadLineCategories))
                ;
            
            CreateMap<AddEditAdditionalSettingCommand, GetFestivalDetailResponse>().ReverseMap();
            CreateMap<AddEditFestivalFileCommand, GetFestivalFileDetailResponse>().ReverseMap();
            CreateMap<FestivalImageDto, GetAllFestivalImageResponse>().ReverseMap();
        }
    }
}