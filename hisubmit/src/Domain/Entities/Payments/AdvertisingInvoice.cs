using System;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Advertise;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Domain.Entities.Payments;

public class AdvertisingInvoice : AuditableEntity<int>
{
    public string InvoiceNumber { get; set; }
    public int FestivalId { get; set; }
    public Festival Festival { get; set; }
    public int? AdvertiseRequestId { get; set; }
    public AdvertiseRequest AdvertiseRequest { get; set; }
    public int? FestivalSettlementStatementId { get; set; }
    public FestivalSettlementStatement FestivalSettlementStatement { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime IssuedOn { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime? PaidOn { get; set; }
    public string PaymentReference { get; set; }
}
