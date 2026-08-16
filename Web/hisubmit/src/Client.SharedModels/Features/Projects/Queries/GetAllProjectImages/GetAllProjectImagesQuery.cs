using Hisubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAllProjectImages;

public class GetAllProjectImagesQuery:PagedRequest
{
    public int  ProjectId { get; set; }
}


public class GetAllProjectImageResponse
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string State { get; set; }
    public int ProjectId { get; set; }
}