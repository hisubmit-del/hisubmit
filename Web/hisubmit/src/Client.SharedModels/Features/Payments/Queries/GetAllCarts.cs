using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace Hisubmit.Client.SharedModels.Features.Payments.Queries;

public class GetAllCartsFilterDto:PagedRequest
{
    public int? FestivalId { get; set; }
    public bool? Paid { get; set; }
    public string UserId { get; set; }
    public NumberFilter<decimal> PriceFilter { get; set; } = new();
    public DateFilter PaidDateFilter { get; set; } = new();
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
    public string PaypalEmail { get; set; }
    public bool TakeCurrentUserCarts { get; set; }
        
}

public class GetAllCartsResponse
{
    public int Id { get; set; }
    public bool Paid { get; set; }
    public decimal Price { get; set; }
    public string UserId { get; set; }
    public string UserFullName { get; set; }
    public bool ShowDetails { get; set; }
    public string PaymentId { get; set; }
    public string PayerId { get; set; }
    public string Email { get; set; }
    
    public DateTime CartDate { get; set; }
        
    public  List<GetCartItemResponse> CartItems { get; set; } = new();
}