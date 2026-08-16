using System.ComponentModel.DataAnnotations;

namespace HiSubmit.Domain.Enums
{
    public enum Questiontype : byte
    {
        [Display(Name = "Text")]
        Text=1,
        [Display(Name = "DropDown menu")]
        DropDownMenu=2,
        [Display(Name = "check box")]
        CheckBox=3,
        [Display(Name = "True or False")]
        True_False=4,
        [Display(Name = "Textarea")]
        TextArea = 5,
    }
}

