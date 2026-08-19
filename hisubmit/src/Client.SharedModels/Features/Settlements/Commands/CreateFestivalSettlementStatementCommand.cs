using System;

namespace Hisubmit.Client.SharedModels.Features.Settlements.Commands;

public class CreateFestivalSettlementStatementCommand
{
    public int FestivalId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}
