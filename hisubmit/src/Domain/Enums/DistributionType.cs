using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums
{
    public enum DistributionType:byte
    {
        [Display(Name = "Distribuator")]
        Distribuator = 1,
        [Display(Name = "Sale agent")]
        SaleAgent = 2
    }

}

