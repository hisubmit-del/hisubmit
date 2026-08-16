using System;

namespace HiSubmit.Application.Models.Emails;

public class SendOpenDateNotificationEmailViewModel
{
    public  string FestivalName { get; set; }
    public  string Title { get; set; }
    public  string FestivalId { get; set; }
    public  DateTime OpenDate { get; set; }
}