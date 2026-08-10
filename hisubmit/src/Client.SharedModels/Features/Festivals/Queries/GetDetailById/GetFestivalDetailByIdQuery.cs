namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;

public class GetFestivalDetailByIdQuery 
{
    public int FestivalId { get; set; }
    public bool WithInclude { get; set; }
    public  string FestivalUrl { get; set; }
}