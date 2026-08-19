namespace HiSubmit.Application.Features.FestivalPaymentItems.Queries.FestivalPaymentStates;

public class GetFestivalPaymentStateResponse
{
    public  int FestivalId { get; set; }
    public  decimal Product { get; set;}
    public  decimal Ticket { get; set; }
    public  decimal Submit { get; set; }
    public  decimal AdminPayment { get; set; }
    public  decimal FestivalDebt { get; set; }
    public  decimal LastMonthIncome { get; set; }
    public  decimal Income { get; set; }
    public decimal SiteCharges { get; set; }
    public decimal NetSettlementDue { get; set; }
}
