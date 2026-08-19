using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Payments;

public class SettlementAdjustment : AuditableEntity<int>
{
    public int FestivalSettlementStatementId { get; set; }
    public FestivalSettlementStatement FestivalSettlementStatement { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string EvidenceUrl { get; set; }
}
