using MediatR;

namespace HiSubmit.Application.Events.ProductSold.AddProductSold;

public class ProductSoldAddedEvent:INotification
{
    public  decimal Price { get; set; }
    public  int ProductSoldId { get; set; }
    public  string ProductName { get; set; }
    public  string ProductImageUrl { get; set; }
}