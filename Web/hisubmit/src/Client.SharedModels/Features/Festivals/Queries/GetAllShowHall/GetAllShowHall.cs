using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;

namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;

public class GetAllShowHallQuery
{
    public int VenueId { get; set; }
}



public class GetAllShowHallResponse
{
    public  int Id { get; set; }
    public  string Title { get; set; }
    public  int Capacity { get; set; }
    public  int AvailableCapacity { get; set; }
        
    public  int VenueId { get; set; }
        
    public  List<ShowTimeDto> ShowTimes { get; set; }

    public GetAllShowHallResponse()
    {
        ShowTimes = new List<ShowTimeDto>();
    }
}
