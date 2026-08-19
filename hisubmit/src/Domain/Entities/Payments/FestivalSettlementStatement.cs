using System;
using System.Collections.Generic;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums.Payments;

namespace HiSubmit.Domain.Entities.Payments;

public class FestivalSettlementStatement : AuditableEntity<int>
{
    public int FestivalId { get; set; }
    public Festival Festival { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossIncome { get; set; }
    public decimal SiteCharges { get; set; }
    public decimal AdvertisingCharges { get; set; }
    public decimal PaymentsToFestival { get; set; }
    public decimal NetAmount { get; set; }
    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;
    public string DisputeReason { get; set; }
    public string ApprovalNote { get; set; }
    public string PaymentReference { get; set; }
    public DateTime? ConfirmedOn { get; set; }
    public DateTime? PaidOn { get; set; }
    public List<SettlementAdjustment> Adjustments { get; set; } = new();
    public List<AdvertisingInvoice> AdvertisingInvoices { get; set; } = new();
}
