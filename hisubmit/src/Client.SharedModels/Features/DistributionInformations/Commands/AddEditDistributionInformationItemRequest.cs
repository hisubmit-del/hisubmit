using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;

public class AddEditDistributionInformationItemRequest
{
    public int Id{ get; set; }
    public int CountryId { get; set; }
    public List<int> MediaRightIds { get; set; }
    public int DistributionInformationId { get; set; }
    public string CountryName { get; set; }

    public AddEditDistributionInformationItemRequest()
    {
        MediaRightIds = new List<int>();
    }
}