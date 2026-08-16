using System;

namespace HiSubmit.Application.Models.Emails;

public class SendNotificationDateEmailViewModel
{
    public  int FestivalId { get; set;}
    public DateTime NotificationDate { get; set; }
    public  string FestivalName { get; set; }
    public  string Title { get; set; }
}