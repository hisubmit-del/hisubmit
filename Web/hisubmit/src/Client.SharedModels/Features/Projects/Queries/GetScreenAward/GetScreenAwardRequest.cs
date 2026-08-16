namespace Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;

public class GetScreenAwardRequest
{
    public int ProjectId { get; set; }
}

  
public class GetScreenAwardResponse
{
    public  int  Id { get; set; }
    public DateTime ScreeningDate { get; set; }
    public string City { get; set; }
    public string CountryName { get; set; }
    public int CountryId { get; set; }
    public string Premiere { get; set; }
    public string AwardSelection { get; set; }
    public string Title { get; set; }
    //navigation property
    public string ImageUrl { get; set; }
    public int ProjectId { get; set; }
}