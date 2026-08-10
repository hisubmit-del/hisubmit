using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;

public class GetAllTicketQuery:PagedRequest
{
    public int? FestivalId { get; set; }
    public  TicketType? TicketType { get; set; }
    public  string SearchString { get; set; }
    public  bool? GetActiveTicket { get; set; }
    public  bool? IsEnable { get; set; }
}

public class GetAllTicketResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public  string Description { get; set; }
    public DateTime OpenDate { get; set; }
    public  DateTime CloseDate { get; set; }
    public  bool AddManagerPercentage { get; set; }
    public  int Cost { get; set; }
    
    public  DateTime EventDate { get; set; }
    
    public  bool IsEnable { get; set; }
    public  int? VenueId { get; set; }
    public  string VenueName { get; set; }
    public  VenueType VenueVenueType { get; set; }
    
    public  string VenueAddress { get; set; }
    
    public  TicketType TicketType { get; set; }
    public  int AvailableCapacity { get; set; }
}