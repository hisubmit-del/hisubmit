namespace Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;

public class GetTicketByIdQuery
{
    public  int Id { get; set; }
    public  int FestivalId { get; set; }
}
public class GetTicketByIdResponse
{
    public int Id { get; set; }
    public  int FestivalId { get; set; }
    public string Title { get; set; }
    public DateTime? OpenDate { get; set; }
    public DateTime? CloseDate { get; set; }
    public bool AddManagerPercentage { get; set; }
    public int Cost { get; set; }

    public  DateTime? EventDate { get; set; }
    public int VenueId { get; set; }
    public  int Capacity { get; set; }
    public string Description { get; set; }
    
    public List<int> ShowHallId { get; set; }

    public HashSet<int> ShowTimesId { get; set; }

    public GetTicketByIdResponse()
    {
        ShowHallId = new List<int>();
        ShowTimesId = new HashSet<int>();
    }

}