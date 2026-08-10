using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Domain.Entities.Content;

public class Comment : AuditableEntity<int>
{
    public string Text { get; set; }
    public string Title { get; set; }
    public bool ShowInSite { get; set; }
    public CommentType Type { get; set; }
    public bool ShowFestival { get; set; }

    [ForeignKey(nameof(Festival))]
    public int? FestivalId { get; set; }
    public Festival Festival { get; set; }

    public  string UserId { get; set; }
    
    [ForeignKey("Parent")]
    public int? ParentId { get; set; }
    public Comment Parent { get; set; }
}