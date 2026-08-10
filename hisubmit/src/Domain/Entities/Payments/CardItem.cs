using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals.Tickets;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Payments;

public class CarTItem : AuditableEntity<int>
{
    public string Title { get; set; }
    public string ItemId { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceAfterDiscount { get; set; }


    public  string Description { get; set; }
    public  string ImageUrl { get; set; }

    public Cart Cart { get; set; }
    public int CartId { get; set; }
    
    public  int? SubmitId { get; set; }
    public Submit Submit { get; set; }

    public int? ProductSoldId { get; set; }
    public ProductSold ProductSold { get; set; }
    
    public  int? SoldTicketId { get; set; }
    public  SoldTicket SoldTicket { get; set; }
    
    public CartItemType CartItemType { get; set; }

    [ForeignKey(nameof(DiscountCode))]
    public int? DiscountCodeId { get; set; }

    public DiscountCode DiscountCode { get; set; }

    public decimal GetRealPrice=>PriceAfterDiscount ?? Price;
}
