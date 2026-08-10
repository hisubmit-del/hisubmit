namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;

public class GetAwardDetailRequest
{
    public int ProjectId { get; set; }
}
    
public class GetAwardDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string AwardsWon { get; set; }
    public string ImageUrl { get; set; }
    public DateTime Date { get; set; }

    public int ProjectId { get; set; }
}