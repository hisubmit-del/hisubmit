using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Festivals;

public class Review:AuditableEntity<int>

{
    public int Rate { get; set; }
    public string Text { get; set; }
    public int FestivalId { get; set; }
    public Festival Festival { get; set;}
    public string UserId { get; set; }
    public string ClientIp { get; set; }
    public CommentType Type { get; set; }
    
    public  int? ParentId { get; set; }
    public Review Parent { get; set; }
}



public class Like : AuditableEntity<int>
{
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public int? NewId { get; set; }
    public New New { get; set; }
    public Festival Festival { get; set; }

}

