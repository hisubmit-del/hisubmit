using System.ComponentModel.DataAnnotations;

namespace Hisubmit.Client.SharedModels.Enums;

public enum TicketType:byte
{
    [Display(Name = "Ticket")]
    Ticket=0,
    [Display(Name = "Badge")]
    Badge=1
}

public enum RefereeStatus
{
    [Display(Name = "Default")]
    Default=0,
    [Display(Name = "Remove From FestivalId")]
    RemoveFromFestival=1,
    [Display(Name = "Remove From Project")]
    RemoveFromProject=2
}