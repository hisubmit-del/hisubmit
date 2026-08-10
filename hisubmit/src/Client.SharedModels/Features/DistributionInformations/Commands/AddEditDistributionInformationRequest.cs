using Hisubmit.Client.SharedModels.Enums;
using System.Collections.Generic;

namespace Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;

public class AddEditDistributionInformationRequest
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int ProjectId { get; set; }
    public DistributionType? DistributionType { get; set; }
    public List<AddEditDistributionInformationItemRequest> Items { get; set; }
    public AddEditDistributionInformationRequest()
    {
        Items = new List<AddEditDistributionInformationItemRequest>();
    }
}