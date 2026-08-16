using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities;

public class Notification:AuditableEntity<int>
{
    public  bool Seen { get; set; }
    public  string Link { get; set; }
    public  string Title { get; set; }
    public  string UserId { get; set; }
    public  int? FestivalId { get; set; }
    public  Festival Festival { get; set; }
    public  SiteAccountType SiteAccountType { get; set; }
    public  NotificationType NotificationType { get; set; }
}

