namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.CheckPermissionForJudging;

public record CheckPermissionForJudgingQuery(string projectURL)
{

}

public class CheckPermissionResponse
{

    public bool Allowed
    {
        get
        {
            return Judgings.Any();
        }
    }
    public List<ProjectJudgingDto> Judgings { get; set; }
}
public class ProjectJudgingDto
{
    public string FestivalName { get; set; }
    public int FestivalId { get; set; }
    public int SubmitId { get; set; }
    public int Id { get; set; }
}