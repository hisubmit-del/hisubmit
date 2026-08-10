using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Locations;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects;

public class DistributionInformationItem:AuditableEntity<int>
{

    public int CountryId { get; set; }
    public Country Country { get; set; }
    public int DistributionInformationId { get; set; }
    public DistributionInformation DistributionInformation { get; set; }
    public List<MediaRightDistributionInformation> MediaRightDistributionInformation { get; set; }


    public DistributionInformationItem()
    {
        MediaRightDistributionInformation = new List<MediaRightDistributionInformation>();
    }

}