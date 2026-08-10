using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

public class DiscountCodesDto
{
    public int Id { get; set; }
    public int? FestivalId { get; set; }
    public string CartItemTypes { get; set; }
    public DateTime? ExpiredTime { get; set; }
    public short? Count { get; set; }
    public DiscountValueType DiscountValueType { get; set; }
    public double DiscountValue { get; set; }
    public string Code { get; set; }
    public bool Enable { get; set; }
    public string Description { get; set; }
}

public class AddEditDiscountCodeRequest : DiscountCodesDto
{
    public bool ForSubmissions { get; set; }
    public bool ForProducts { get; set; }
    public bool ForTickets { get; set; }
}

public class GetAllDiscountCodeResponse : DiscountCodesDto;

public class DiscountCodeFilter : PagedRequest
{
    public int? FestivalId { get; set; }
    public bool? Enable { get; set; }
}

public class ChangeDiscountCodeStatusRequest
{
    public int Id { get; set; }
    public bool Enable { get; set; }
    public int? FestivalId { get; set; }
}