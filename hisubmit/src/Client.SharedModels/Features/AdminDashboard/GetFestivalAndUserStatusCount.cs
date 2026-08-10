using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;

namespace Hisubmit.Hisubmit.Client.SharedModels.Features.AdminDashboard;

public class GetFestivalAndUserStatusCount
{
    public int AllAccountCount { get; set; }
    public int FestivalCount { get; set; }
    public int ActiveFestivalCount { get; set; }
    public int SubmitCount { get; set; }
    public int ProjectsCount { get; set; }
}

public class GetSitePurchaseResponse
{
    public decimal AllInComes => Submission + AllProduct + ServiceFee + AllTicket;
    public decimal AllSiteInComes => ServiceFee + SiteTicket + SiteProduct;

    public decimal Submission { get; set; }
    public decimal ServiceFee { get; set; }

    public decimal AllProduct { get; set; }
    public decimal SiteProduct { get; set; }

    public decimal AllTicket { get; set; }
    public decimal SiteTicket { get; set; }
    public decimal GoldAccount { get; set; }
    
}

public class GetSitePurchaseRequest
{
    public DateFilter DateFilter { get; set; } = new();
}