using AutoMapper;
using HiSubmit.Application.Features.FestivalPaymentItems.Commands.Add;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentItems.Queries.GetDetail;
using HiSubmit.Application.Features.Payments.Commands.EditSiteCommission;
using HiSubmit.Application.Features.Payments.DiscountsCodes.Commands;
using HiSubmit.Application.Features.Payments.Queries;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Application.Mappings;

public class PaymentProfile:Profile
{
    public PaymentProfile()
    {
        CreateMap<SiteCommission, GetSiteCommissionResponse>().ReverseMap();
        CreateMap<SiteCommission, EditSiteCommissionCommand>().ReverseMap();

        CreateMap<AddToCartRequest, CarTItem>().ReverseMap();
        
        // CreateMap<GetAllCartsResponse, Cart>()
        //     .ForMember(p=>p.CartItems).ReverseMap();

        CreateMap<AddFestivalPaymentItemCommand, FestivalPaymentItem>().ReverseMap();
        CreateMap<GetAllFestivalPaymentItemResponse, FestivalPaymentItem>().ReverseMap();
        CreateMap<GetFestivalPaymentItemDetailResponse, FestivalPaymentItem>().ReverseMap();

        CreateMap<AddEditDiscountCodeCommand, DiscountCode>()
            .ReverseMap();

        CreateMap<GetAllDiscountCodeResponse,DiscountCode>()
            .ReverseMap();

        CreateMap<DiscountCode,DiscountCodesDto>()
            .ReverseMap();
    }
}