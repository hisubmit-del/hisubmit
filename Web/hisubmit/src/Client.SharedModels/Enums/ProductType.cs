using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum ProductType:int
{
    [Display(Name = "Downloadable Product")]
    Downloadable=0,
    [Display(Name = "Product Sent")]
    Sent=1
}