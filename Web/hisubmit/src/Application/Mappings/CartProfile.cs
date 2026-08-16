using System;
using System.Linq;
using AutoMapper;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums;
using Queries_GetCartItemResponse = Hisubmit.Client.SharedModels.Features.Payments.Queries.GetCartItemResponse;

namespace HiSubmit.Application.Mappings;

public class CartProfile : Profile
{
    public CartProfile()
    {
        CreateMap<Queries_GetCartItemResponse, CarTItem>()
            .ReverseMap()
            .ForMember(des => des.CartItemType, map => map.MapFrom(src => src.CartItemType))
            .ForMember(des => des.Paid, map => map.MapFrom(src => src.Cart.Paid))
            .ForMember(des => des.PaidDate, map => map.MapFrom(src => src.Cart.CartDate))
            .ForMember(des => des.ProductId, map => map.MapFrom(src => src.ProductSoldId))
            .ForMember(des => des.ProductName, map => { map.MapFrom(src => src.ProductSold!.Product!.Name); })
            .ForMember(des => des.SoldTicketName, map => { map.MapFrom(p => p.SoldTicket!.Ticket!.Title); })
            .ForMember(des => des.ProjectName, map =>
                map.MapFrom(p => p.Submit!.Project!.Title))
            .ForMember(des=>des.SubmitCategoriesName,map=>map.MapFrom(src=>string.Join(',',src.Submit!.SubmitDeadlineEventCategories!
                .Select( p=>p.DeadlineEventCategory!.EventCategory!.Name))))
            .ForMember(des=>des.FestivalId,map=>map.MapFrom(src=>GetFestivalId(src)))
            .ForMember(des=>des.FestivalName,map=>map.MapFrom(src=>GetFestivalName(src)))
            .ForMember(des => des.ProductSoldShareFestival, map =>
                map.MapFrom(p => p.ProductSold!.ShareFestivalIncome))
            .ForMember(des => des.SoldTicketShareFestival, map =>
                map.MapFrom(p => p.SoldTicket!.ShareFestivalIncome))
            .ForMember(des => des.SubmitTrackCode, map =>
                map.MapFrom(p => p.Submit!.TrackingCode));

        CreateMap<GetAllCartsResponse, Cart>()
            .ReverseMap();
        
    }

    private static int? GetFestivalId(CarTItem item)
    {
        switch (item.CartItemType)
        {
            case CartItemType.Submit:
                return item.Submit.FestivalId;
            case CartItemType.Badge:
                return item.SoldTicket!.Ticket!.Venue!.FestivalId;
            case CartItemType.Ticket:
                return item.SoldTicket!.Ticket!.Venue!.FestivalId;
            case CartItemType.SpecialAccount:
                return null;
            case CartItemType.ServiceFee:
                return item.Submit.FestivalId;
            case CartItemType.Product:
                return item.ProductSold.Product.FestivalId;
        }
        return null;
    }
    
    private static string GetFestivalName(CarTItem item)
    {
        switch (item.CartItemType)
        {
            case CartItemType.Submit:
                return item.Submit.Festival.Name;
            case CartItemType.Badge:
                return item.SoldTicket!.Ticket!.Venue!.Festival!.Name;
            case CartItemType.Ticket:
                return item.SoldTicket!.Ticket!.Venue!.Festival!.Name;
            case CartItemType.SpecialAccount:
                return "Hisubmit";
            case CartItemType.ServiceFee:
                return item.Submit.Festival.Name;
            case CartItemType.Product:
                return item.ProductSold.Product.Festival!.Name;
        }
        return null;
    }
}
