using Hisubmit.Client.SharedModels.Enums;
using System;

namespace Hisubmit.Client.SharedModels.Features.Payments.Queries;

public class GetCartItemResponse
{
    public int Id { get; set; }
    public bool Paid { get; set; }
    public int CartId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public decimal? PriceAfterDiscount { get; set; }

    public string ItemId { get; set; }
    public string ImageUrl { get; set; }

    public string SubmitCategoriesName { get; set; }
    public DateTime PaidDate { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }
    public CartItemType CartItemType { get; set; }
    public int? FestivalId { get; set; }
    public string FestivalName { get; set; }
    public bool CanDelete => !Paid && CartItemType != CartItemType.ServiceFee;

    public int? SubmitId { get; set; }
    public string ProjectName { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; }
    public short ProductCount { get; set; }
    public decimal? ProductSoldShareFestival { get; set; }
    public int? SoldTicketId { get; set; }
    public decimal? SoldTicketShareFestival { get; set; }
    public string SoldTicketName { get; set; }

    public decimal FestivalShare { get; set; }
    public string SubmitTrackCode { get; set; }


    public int? DiscountCodeId { get; set; }
    public string DiscountCode { get; set; }


    public decimal GetRealPrice()
    {
        if (PriceAfterDiscount == null)
            return Price;
        return PriceAfterDiscount.Value;
    }

    public string GetTitle()
    {
        var title = string.Empty;
        switch (this.CartItemType)
        {
            case CartItemType.Submit:
                title = this.ProjectName;
                break;
            case CartItemType.Badge:
                title = this.SoldTicketName;
                break;
            case CartItemType.Ticket:
                title = this.SoldTicketName;
                break;
            case CartItemType.Product:
                title = this.ProductName;
                break;
            case CartItemType.ServiceFee:
                title = this.Title;
                break;
            case CartItemType.SpecialAccount:
                title = this.Title;
                break;
            default:
                title = string.Empty;
                break;
        }

        return title;
    }

    public decimal GetShareFestival()
    {
        if (CartItemType == CartItemType.Submit)
            return Price;
        if (SoldTicketId != null)
            return SoldTicketShareFestival.Value;
        if (ProductId != null)
            return ProductSoldShareFestival.Value;
        return 0;
    }

}

public class CalculateDiscountCodesRequest
{
    public int CartId { get; set; }
    public List<string> DiscountCodes { get; set; }
}
