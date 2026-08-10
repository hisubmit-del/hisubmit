using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums
{
    public enum DistributionType:byte
    {
        [Display(Name = "Distribuator")]
        Distribuator = 1,
        [Display(Name = "Sale agent")]
        SaleAgent = 2
    }

}

