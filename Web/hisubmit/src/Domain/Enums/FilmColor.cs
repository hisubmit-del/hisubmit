using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums
{
    public enum FilmColor : byte
    {
        [Display(Name ="Color")]
        Color=0,
        [Display(Name ="Black & White")]
        Black_White = 1,
        [Display(Name ="Black & White & Color")]
        Black_White_Color=2

    }

}

