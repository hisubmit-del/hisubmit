
namespace Hisubmit.Client.SharedModels.Features.Settlements.Commands;

public class AddSettlementAdjustmentCommand
{
    public int StatementId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string EvidenceUrl { get; set; }
}
