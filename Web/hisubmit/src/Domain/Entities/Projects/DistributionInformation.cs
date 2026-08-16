using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums;
using System.Collections.Generic;

namespace HiSubmit.Domain.Entities.Projects;

public class DistributionInformation:AuditableEntity<int>
{
    public string Title { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public DistributionType? DistributionType { get; set; }
    public List<DistributionInformationItem> Items { get; set; }

    public DistributionInformation()
    {
        Items = new List<DistributionInformationItem>();
    }
}