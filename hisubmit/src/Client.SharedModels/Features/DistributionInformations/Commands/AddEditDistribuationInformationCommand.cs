namespace Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;

public class UpdateDistributionInformationCommand 
{
    public int ProjectId { get; set; }
    public List<AddEditDistributionInformationRequest> Information { get; set; }
    
    public UpdateDistributionInformationCommand()
    {
        Information = new List<AddEditDistributionInformationRequest>();
    }
}