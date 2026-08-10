using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Projects;
public class ProjectImage:AuditableEntity<int>
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string State { get; set; }
    public int ProjectId { get; set; }
    public  Project Project { get; set; }
}