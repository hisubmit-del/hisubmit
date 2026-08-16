using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Notifications.Queries;

public class GetAllNotificationQuery : PagedRequest
{
    public bool? Seen { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public SiteAccountType SiteAccountType { get; set; }
}

public class GetAllNotificationResponse
{
    public int Id { get; set; }
    public bool Seen { get; set; }
    public string Link { get; set; }
    public string Title { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public SiteAccountType SiteAccountType { get; set; }
    public NotificationType NotificationType { get; set; }
    public DateTime CreatedOn { get; set; }
}