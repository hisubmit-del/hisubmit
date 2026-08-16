using HiSubmit.Domain.Enums.Festivals;

namespace HiSubmit.Application.Features.FestivalPaymentsInformation.Queries.GetDetail;

public class GetFestivalPaymentInformationDetailResponse
{
    public  int Id { get; set; }
    public FestivalPaymentType Type { get; set; }
    public string PaypalEmail { get; set; }
    public string CardNumber { get; set; }
    public string CVC { get; set; }
    public string Expires { get; set; }
    
    public  int FestivalId { get; set; }
    public  string FestivalName { get; set; }
    public  string FestivalLogoUrl { get; set; }
}