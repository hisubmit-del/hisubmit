using HiSubmit.Domain.Contracts;

namespace HiSubmit.Domain.Entities.Payments;

public class SiteCommission:AuditableEntity<int>
{
    public double SubmitServiceFee { get; set; }
    public double MinimumServiceFee { get; set; }
    public double MaximumServiceFee { get; set; }

    public double UsualFestivalCommission { get; set; }
    public double SpecialFestivalCommission { get; set; }

    public double TicketSalesCommission { get; set; }
    public double ProductSalesCommission { get; set; }
    
    
    public double  MonthlySpecialUserFee { get; set; } 
    public double ThreeMonthlySpecialUserFee { get; set; }
    public  double YearlySpecialUserFee { get; set; }
}

