using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Catalog;

public class FestivalQualifying:AuditableEntity<int>
{
    public string Name { get; set; }
    public string LogoName { get; set; }
}