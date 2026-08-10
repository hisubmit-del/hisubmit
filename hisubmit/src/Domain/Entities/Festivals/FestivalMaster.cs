using System.Collections.Generic;
using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Festivals;

public class FestivalMaster:AuditableEntity<int>
{
    public string Name { get; set; }
    public int ActivePeriod { get; set; }
    public int ActiveId { get; set; }
    public List<Festival> Festivals { get; set; }
    
    public  string UserId { get; set; }
}