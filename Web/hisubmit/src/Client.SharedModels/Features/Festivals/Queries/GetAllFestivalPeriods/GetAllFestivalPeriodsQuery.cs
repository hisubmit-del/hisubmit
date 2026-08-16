namespace Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalPeriods;

public class GetAllFestivalPeriodsQuery
{
    public int FestivalId { get; set; }
    public int FestivalMasterId { get; set; }
}

public class GetAllFestivalPeriodsResponse
{
    public int FestivalMasterId { get; set; }
    public List<FestivalPeriod> FestivalPeriods { get; set; }

    public GetAllFestivalPeriodsResponse()
    {
        FestivalPeriods = new List<FestivalPeriod>();
    }
}

public class FestivalPeriod
{
    public int FestivalId { get; set; }
    public int Period { get; set; }
}