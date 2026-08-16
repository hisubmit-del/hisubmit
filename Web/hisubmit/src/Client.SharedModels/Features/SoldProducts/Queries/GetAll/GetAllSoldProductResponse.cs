using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.SoldProducts.Queries;

public class GetAllSoldProductResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public int ProductId { get; set; }
    public string UserId { get; set; }
    public string ProductName { get; set; }

    public decimal ShareFestivalIncome { get; set; }
    public decimal Income { get; set; }
    public decimal ProductPrice { get; set; }

    public ProductType ProductType { get; set; }
    public ProductSoldStatus Status { get; set; }
}