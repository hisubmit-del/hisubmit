using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetProjectSpecifications;

public class GetProjectSpecificationQuery
{
    public int Id { get; set; }
}

public class GetProjectSpecificationResponse
{
    public bool StudentProject { get; set; }
    public ProjectType ProjectType { get; set; }
    public  int Size { get; set; }
}