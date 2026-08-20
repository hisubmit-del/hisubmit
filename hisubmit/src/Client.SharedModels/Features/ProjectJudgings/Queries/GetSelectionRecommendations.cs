using System;
using System.Collections.Generic;
using Hisubmit.Client.SharedModels.Enums;

namespace Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries;

public class GetSelectionRecommendationsRequest
{
    public int? ProjectType { get; set; }
    public int MinimumReviews { get; set; } = 1;
}

public class SelectionRecommendationResponse
{
    public int SubmitId { get; set; }
    public int ProjectId { get; set; }
    public string ProjectTitle { get; set; }
    public string TrackingCode { get; set; }
    public ProjectType ProjectType { get; set; }
    public int ReviewCount { get; set; }
    public int CompletedReviewCount { get; set; }
    public double AverageScore { get; set; }
    public DateTime SubmittedOn { get; set; }
    public JudgingStatus JudgingStatus { get; set; }
    public string Recommendation { get; set; }
    public string Explanation { get; set; }
}
