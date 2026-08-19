using System;

namespace Hisubmit.Client.SharedModels.Features.Settlements.Commands;

public class CreateAdvertisingInvoiceCommand
{
    public int FestivalId { get; set; }
    public int? AdvertiseRequestId { get; set; }
    public int? StatementId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
    public DateTime? DueOn { get; set; }
}
