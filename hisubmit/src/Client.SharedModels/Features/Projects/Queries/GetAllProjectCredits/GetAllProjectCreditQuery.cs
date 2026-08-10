using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectCreditCommand;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectCredits;

public class GetAllProjectCreditQuery
{
    public int ProjectId { get; set; }
    public bool WithInclude { get; set; }
}


 
public class GetAllProjectCreditResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public List<AddEditProjectCreditItemCommand> ProjectItemPeople { get; set; }
    public int ProjectId { get; set; }

    public GetAllProjectCreditResponse()
    {
        ProjectItemPeople = new List<AddEditProjectCreditItemCommand>();
    }
}