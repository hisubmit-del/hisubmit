using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllSubProjectType;

public class GetAllSubProjectSelectedTypeQuery
{
    public int ProjectId { get; set; }
    public  ProjectType ProjectType { get; set; }
}
public class GetAllSubProjectTypeResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class SubProjectSpecficationDto
{
    public int ProjectTypeId { get; set; }
}
