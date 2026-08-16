using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums
{
    [DefaultValue(DontPaid)]
    public enum SubmitStatus:byte
    {
        [Display(Name = "Not set")]
        Default=0,
        [Display(Name = "UnPaid")]
        DontPaid=1,
        [Display(Name = "Paid")]
        Paid=2,
        [Display(Name = "Inconsideration")]
        Inconsideration=3,
        [Display(Name = "InComplete")]
        InComplete=4,
        [Display(Name = "Disqualified")]
        Disqualified=5,
        [Display(Name = "Withdrawn")]
        Withdrawn=6
    }

    [DefaultValue(NotSelected)]
    public enum JudgingStatus : byte
    {
        [Display(Name = "Undecided")]
        Undecided=0,
        [Display(Name = "Not Selected")]
        NotSelected=2,
        [Display(Name = "Selected")]
        Selected=3,       
        [Display(Name = "Award Winner")]
        AwardWinner=4,
        [Display(Name = "Finalist")]
        Finalist=5,
        [Display(Name = "Semi Finalist")]
        SemiFinalist=6,
        [Display(Name = "Quarter Finalist")]
        QuarterFinalist=7,        
        [Display(Name = "Nominee")]
        Nominee=8,        
        [Display(Name = "Honorable Mention")]
        HonorableMention=9
    }
}

