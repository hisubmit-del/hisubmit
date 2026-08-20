using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums
{
    public enum Gender : byte
    {
        [Display(Name = "Male")]
        Male=0,
        [Display(Name = "Female")]
        Female=1,
        [Display(Name = "Other")]
        Other=2
    }

    public enum VenueType:int
    {
        [Display(Name = "Office")]
        Office=0,
        [Display(Name = "Show Location")]
        ShowLocation=1,
        [Display(Name = "Gallery")]
        Gallery=2,
        [Display(Name = "Market")]
        Market=3,
        [Display(Name = "Conference")]
        Conference=4,
        [Display(Name = "Other")]
        Other=5
    }
}

