using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;


namespace Hisubmit.Client.SharedModels.Interfaces.Carts;
public class AddToCartRequest
{
    public string Title { get; set; }
    public string ItemId { get; set; }
    public decimal Price { get; set; }

    public  string Description { get; set; }
    public  string ImageUrl { get; set; }
    
    public  int? SubmitId { get; set; }

    public int? ProductSoldId { get; set; }
    
    public  int? SoldTicketId { get; set; }
    
    public CartItemType CartItemType { get; set; }
}
