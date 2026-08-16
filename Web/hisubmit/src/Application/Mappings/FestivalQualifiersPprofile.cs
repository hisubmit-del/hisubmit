using AutoMapper;
using HiSubmit.Application.Features.FestivalQualifyers.Queries.GetAll;
using HiSubmit.Domain.Entities.Catalog;

namespace HiSubmit.Application.Mappings
{
    public class FestivalQualifiersPprofile : Profile
    {
        public FestivalQualifiersPprofile()
        {
            CreateMap<FestivalQualifying, GetAllFestivalQualifiersResponse>().ReverseMap();
        }
    }

}