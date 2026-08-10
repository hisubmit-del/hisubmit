using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums;

public enum ProductType:int
{
    [Display(Name = "Downloadable Product")]
    Downloadable=0,
    [Display(Name = "Product Sent")]
    Sent=1
}