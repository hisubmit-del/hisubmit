using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums
{
    public enum ProjectType : byte
    {
        [Display(Name ="Film")]
        Film=1,
        [Display(Name ="Photography")]
        Photography=2,
        [Display(Name ="Music")]
        Music=3,
        [Display(Name ="Script/ScreenWriting")]
        Script_ScreenWriting=4,
        [Display(Name ="Vr/Xr/Immersive")]
        VR_XR=5,
        [Display(Name = "Art")]
        Art=6
    }
}

