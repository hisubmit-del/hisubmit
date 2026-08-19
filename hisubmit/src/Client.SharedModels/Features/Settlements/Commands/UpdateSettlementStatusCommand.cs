using Hisubmit.Client.SharedModels.Enums.Payments;

namespace Hisubmit.Client.SharedModels.Features.Settlements.Commands;

public class UpdateSettlementStatusCommand
{
    public int StatementId { get; set; }
    public SettlementStatus Status { get; set; }
    public string Note { get; set; }
    public string PaymentReference { get; set; }
}
