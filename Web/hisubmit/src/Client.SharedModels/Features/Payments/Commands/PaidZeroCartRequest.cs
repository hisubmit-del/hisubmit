namespace Hisubmit.Client.SharedModels.Features.Payments.Commands;

public class PaidZeroCartRequest
{
    public int Id { get; set; }
}


public class CheckPaymentResponse
{
    public string PayerId { get; set; }
    public string PaymentId { get; set; }
    public string OrderId { get; set; }
}