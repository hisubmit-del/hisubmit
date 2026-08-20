using System;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Client.SharedModels.Wrapper;

namespace Hisubmit.Client.SharedModels.Features.Recommendations.Queries;

public class GetGoldFestivalRecommendationsRequest
{
    public int ProjectId { get; set; }
}

public sealed class GoldFestivalRecommendation
{
    public int FestivalId { get; set; }
    public string FestivalName { get; set; }
    public string FestivalUrl { get; set; }
    public ProjectType EventType { get; set; }
    public DateTime NextDeadline { get; set; }
    public bool HasGoldFee { get; set; }
    public int MatchScore { get; set; }
    public string MatchReason { get; set; }
}
