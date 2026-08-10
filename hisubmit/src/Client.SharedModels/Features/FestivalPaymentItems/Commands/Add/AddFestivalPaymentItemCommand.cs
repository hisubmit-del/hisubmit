using Hisubmit.Client.SharedModels.Enums.Festivals;

namespace Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Commands.Add;

public class AddFestivalPaymentItemCommand 
{
    public double Amount { get; set; }
    public int FestivalId { get; set; }
    public DateTime? PaidDate { get; set; }
    public string TrackNumber { get; set; }
    public FestivalPaymentType Type { get; set; }
}

