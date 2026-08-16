using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums
{
    public enum CartItemType:byte
    {
        [Display(Name = "Submit")]
        Submit=1,
        [Display(Name = "Badge")]
        Badge=2,
        [Display(Name = "Ticket")]
        Ticket=3,
        [Display(Name = "Special Account")]
        SpecialAccount=4,
        [Display(Name = "Service Fee")]
        ServiceFee=5,
        [Display(Name = "Product")]
        Product=6
    }
}

