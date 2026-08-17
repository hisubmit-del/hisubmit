using HiSubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Reviews.Queries;

public class GetFestivalRatingSummaryQuery
{
    public int FestivalId { get; set; }
}

public class GetFestivalRatingSummaryResponse
{
    public double AverageRate { get; set; }
    public int TotalVotes { get; set; }
    public bool HasRated { get; set; }
}
