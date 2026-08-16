using AutoMapper;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Queries.GetDetail;
using Hisubmit.Client.SharedModels.Features.Payments.Commands.EditSiteCommission;
using Hisubmit.Client.SharedModels.Features.Payments.Queries;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace HiSubmit.Client.Infrastructure.Mappings;

public class PaymentProfile:Profile
{
    public PaymentProfile()
    {
        CreateMap<EditSiteCommissionCommand, GetSiteCommissionResponse>().ReverseMap();
        CreateMap<AddEditDiscountCodeRequest, GetAllDiscountCodeResponse>().ReverseMap();
        CreateMap<AddEditFestivalPaymentInformationCommand, GetFestivalPaymentInformationDetailResponse>().ReverseMap();
    }
}