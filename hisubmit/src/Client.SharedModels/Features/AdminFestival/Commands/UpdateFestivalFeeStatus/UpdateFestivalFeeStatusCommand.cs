using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;

public class UpdateFestivalFeeStatusRequest 
{
    public int FestivalId { get; set; }
    public FeeStatus FeeStatus { get; set; }
}

