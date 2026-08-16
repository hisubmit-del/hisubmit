using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums
{
    public enum EventType : byte
    {
        [Display(Name ="Film ProductFestivalId")]
        FilmFestival,

        [Display(Name ="Writing and poem")]
        WritingAndPoem,

        [Display(Name = "Music Contest")]
        MusicContest,

        [Display(Name = "Photographic contest")]
        PhotographicContest,

        [Display(Name = "Online ProductFestivalId")]
        OnlineFestival_AwardsEvent
    }
}

