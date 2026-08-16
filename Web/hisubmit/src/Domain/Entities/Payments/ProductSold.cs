using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Domain.Entities.Locations;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Payments;

public class ProductSold:AuditableEntity<int>
{
    public string UserId { get; set; }
    public  string Email { get; set; }

    public  Address Address { get; set; }
    public  int? AddressId { get; set; }

    public Product Product { get; set; }
    public int ProductId { get; set; }

    public decimal Income { get; set; }//مبلغ پرداخت شده کاربر
    public decimal ShareFestivalIncome { get; set; }//سهم فستیوال 

    public  ProductSoldStatus Status { get; set; }
}

