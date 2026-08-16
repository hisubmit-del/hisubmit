namespace Hisubmit.Client.SharedModels.Features.Payments.Commands;

public class PaidCartRequest 
{
    public int CartId { get; set; }
    public string OrderId { get; set; }
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
    public string Email { get; set; }
    public decimal Price { get; set; }
    public List<string> DiscountCodes { get; set; }
}

public class DownloadCartFactorRequest
{
    public int Id { get; set; }
}

public class DownloadCartFactorResponse
{
    public string MimeType { get; set; } 
    public string FileName { get; set; }
    public byte[] File { get; set; }
}

