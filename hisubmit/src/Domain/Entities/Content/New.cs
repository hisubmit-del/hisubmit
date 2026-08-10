using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;

namespace HiSubmit.Domain.Entities.Content;

public class New:AuditableEntity<int>
{
    public  string Title { get; set; }
    public  string BannerUrl { get; set; }
    public  string Description { get; set; }
    public  bool IsEnable { get; set; }
    public string ShortDescription { get; set; }
    
    [ForeignKey(nameof(Festival))]
    public int? FestivalId { get; set; }
    public  Festival Festival { get; set; }
}

