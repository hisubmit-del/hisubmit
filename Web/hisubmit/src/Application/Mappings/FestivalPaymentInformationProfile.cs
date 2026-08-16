using AutoMapper;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Commands.AddEdit;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetAll;
using HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;
using HiSubmit.Domain.Entities.Payments;

namespace HiSubmit.Application.Mappings;

public class FestivalPaymentInformationProfile:Profile
{
    public FestivalPaymentInformationProfile()
    {
        CreateMap<GetAllFestivalPaymentInformationResponse, FestivalPaymentInformation>()
            .ReverseMap();
        CreateMap<AddEditFestivalPaymentInformationCommand, FestivalPaymentInformation>()
            .ReverseMap();
        CreateMap<GetFestivalPaymentInformationDetailResponse, FestivalPaymentInformation>()
            .ReverseMap();
    }
}