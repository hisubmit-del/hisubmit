using System;
using Hisubmit.Client.SharedModels.Enums.Festivals;

namespace Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Queries.GetAll;

public class GetAllFestivalPaymentItemResponse
{
    public int Id { get; set; }
    public  double Amount { get; set; }
    public  int FestivalId { get; set; }
    public DateTime  PaidDate { get; set; }
    public  string FestivalName { get; set; }
    public  string FestivalLogoUrl { get; set; }
    public  string TrackNumber { get; set; }
    public  FestivalPaymentType Type { get; set; }
}
