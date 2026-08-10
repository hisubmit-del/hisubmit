using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects;

public class MediaRightDistributionInformation : AuditableEntity<int>
{
    public int MediaRightId { get; set; }
    public MediaRight MediaRight { get; set; }

    public DistributionInformationItem DistributionInformationItem { get; set; }
    public int DistributionInformationItemId { get; set; }
}