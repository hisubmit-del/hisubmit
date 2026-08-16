namespace HiSubmit.Application.Features.Festivals.Queries.GetAllIncome;

public class GetAllFestivalIncomeResponse
{
    public int FestivalId { get; set; }
    public double TotalPrice { get; set; }
    public double PaidTotlaPrice { get; set; }
    public double UnPaidTotalPrice { get; set; }
}

