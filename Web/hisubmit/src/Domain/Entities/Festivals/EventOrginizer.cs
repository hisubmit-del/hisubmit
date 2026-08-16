using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals;

public class EventOrginizer:AuditableEntity<int>
{
    public string Name { get; set; }
    public string Title { get; set; }

    [ForeignKey(nameof(Festival))]
    public int FestivalId { get; set; }
    public Festival Festival { get; set; }

    public string ImageName { get; set; }
}