using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums
{
    public enum EventType : byte
    {
        [Display(Name ="Film FestivalId")]
        FilmFestival,

        [Display(Name ="Writing and poem")]
        WritingAndPoem,

        [Display(Name = "Music Contest")]
        MusicContest,

        [Display(Name = "Photographic contest")]
        PhotographicContest,

        [Display(Name = "Online FestivalId")]
        OnlineFestival_AwardsEvent
    }
}

