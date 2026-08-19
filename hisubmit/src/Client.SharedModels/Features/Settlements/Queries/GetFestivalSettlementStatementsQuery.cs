using System;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Enums.Payments;

namespace Hisubmit.Client.SharedModels.Features.Settlements.Queries;

public class GetFestivalSettlementStatementsQuery
{
    public int FestivalId { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
}

public class FestivalSettlementStatementResponse
{
    public int Id { get; set; }
    public int FestivalId { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public decimal GrossIncome { get; set; }
    public decimal SiteCharges { get; set; }
    public decimal AdvertisingCharges { get; set; }
    public decimal PaymentsToFestival { get; set; }
    public decimal NetAmount { get; set; }
    public SettlementStatus Status { get; set; }
    public string DisputeReason { get; set; }
    public string ApprovalNote { get; set; }
    public string PaymentReference { get; set; }
    public DateTime? ConfirmedOn { get; set; }
    public DateTime? PaidOn { get; set; }
    public List<SettlementAdjustmentResponse> Adjustments { get; set; } = new();
    public List<AdvertisingInvoiceResponse> AdvertisingInvoices { get; set; } = new();
}

public class SettlementAdjustmentResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; }
    public string EvidenceUrl { get; set; }
}

public class AdvertisingInvoiceResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; }
    public DateTime IssuedOn { get; set; }
    public DateTime? DueOn { get; set; }
    public DateTime? PaidOn { get; set; }
    public string PaymentReference { get; set; }
}

public class SettlementFileResponse
{
    public byte[] File { get; set; }
    public string MimeType { get; set; }
    public string FileName { get; set; }
}
