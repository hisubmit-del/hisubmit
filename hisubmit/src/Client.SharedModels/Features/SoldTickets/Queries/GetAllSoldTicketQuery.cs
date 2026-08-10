using Hisubmit.Client.SharedModels.Wrapper;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;

public class GetAllSoldTicketQuery:PagedRequest
{
    public  string UserId { get; set; }
    public  int? FestivalId { get; set; }
    public  int? TicketId { get; set; }
    public  int? VenueId { get; set; }
    public  string SearchString { get; set; }
    public  SoldTicketStatus? SoldTicketStatus { get; set; }
}

public class GetAllSoldTicketResponse
{
    public  int Id { get; set; }
    public string TicketTitle { get; set; }
    public  TicketType TicketType { get; set; }
    public decimal Cost { get; set; }
    public  string BuyerFullNameName { get; set; }
    public  string BuyerEmail { get; set; }
    public  int Count { get; set; }
    public decimal ShareFestivalIncome { get; set; }
    public  string UserId { get; set; }
    public  int TicketId { get; set; }
    public DateTime? CreatedOn { get; set; }
    public  int? ShowTimeId { get; set; }
    public  bool ForOtherUser { get; set; }
    public  string OtherUserEmail { get; set; }
    public  int? ChairNumber { get; set; }
    public  SoldTicketStatus SoldTicketStatus { get; set; }
}