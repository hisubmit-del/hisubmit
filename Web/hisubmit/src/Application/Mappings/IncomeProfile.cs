using AutoMapper;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Features.Festivals.Queries.GetAllIncome;

namespace HiSubmit.Application.Mappings
{
    public class IncomeProfile:Profile
    {
        public IncomeProfile()
        {
            CreateMap<CarTItem, GetAllFestivalIncomeItem>()
                .ForMember(des => des.UserId, map => map.MapFrom(src => src.Cart.UserId))
                .ForMember(des => des.PaidDate, map => map.MapFrom(src => src.Cart.CartDate))
                .ForMember(des => des.Title, map => map.MapFrom(src => src.Submit.Project.Title))
                .ForMember(des => des.IncomItemType, map => map.MapFrom(src => src.CartItemType));
        }
    }
}


