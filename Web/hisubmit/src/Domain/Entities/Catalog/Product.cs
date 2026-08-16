using System.Collections.Generic;
using HiSubmit.Domain.Contracts;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Entities.Festivals;
using ProductType = HiSubmit.Domain.Enums.ProductType;

namespace HiSubmit.Domain.Entities.Catalog;

public class Product : AuditableEntity<int>
{
    public string Name { get; set; }
    public string Barcode { get; set; }
    [Column(TypeName = "text")]
    public string ImageDataURL { get; set; }
    public string ShortDescription { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public  ProductType ProductType { get; set; }
    public  bool IsEnable { get; set; }

    [ForeignKey(nameof(Festival))]
    public int FestivalId { get; set; }
    [IgnoreDataMember]
    public Festival Festival { get; set; }

    public ShowInSiteStatus Status { get; set; }
    
    public List<ProductImage> ProductImages { get; set; }
}

public class ProductImage : AuditableEntity<int>
{
    public int ProductId { get; set; }
    public Product  Product { get; set; }
    
    public string Url { get; set; }
}

