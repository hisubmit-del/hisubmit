namespace Hisubmit.Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries.GetRefereeData;

public class GetRefereeDataRequest
{
    public string UserId { get; set; }
    public bool GetCurrentUserData { get; set; }
}

public class GetRefereeDataResponse
{
    public int NotRatedProject { get; set; }
    public int RatedProject { get; set; }
    public double AverageRate { get; set; }
}