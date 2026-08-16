using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals;

public class FestivalSubUser : AuditableEntity<int>
{
    public int FestivalId { get; set; }
    public string UserId { get; set; }
    public Festival Festival { get; set; }
    public bool IsReferee { get; set; }
    public bool IsRemoved { get; set; }
}