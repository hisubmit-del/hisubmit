using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.ProjectJudgings.Queries;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.ProjectJudgings.Queries.GetSelectionRecommendations;

public class GetSelectionRecommendationsQuery : GetSelectionRecommendationsRequest,
    IRequest<Result<List<SelectionRecommendationResponse>>>
{
    public int FestivalId { get; set; }
}

public class GetSelectionRecommendationsQueryHandler
    : IRequestHandler<GetSelectionRecommendationsQuery, Result<List<SelectionRecommendationResponse>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public GetSelectionRecommendationsQueryHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<SelectionRecommendationResponse>>> Handle(
        GetSelectionRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FestivalId <= 0)
            return await Result<List<SelectionRecommendationResponse>>.FailAsync(
                "A valid festival is required.");

        var assignments = await _unitOfWork.Repository<ProjectJudging>()
            .Entities
            .AsNoTracking()
            .Include(p => p.Submit)
                .ThenInclude(s => s.Project)
            .Include(p => p.JudgingFiledAnswereds)
            .Where(p => p.Submit.FestivalId == request.FestivalId &&
                        (!request.ProjectType.HasValue ||
                         (int)p.Submit.Project.ProjectType == request.ProjectType.Value))
            .ToListAsync(cancellationToken);

        var recommendations = assignments
            .GroupBy(p => p.SubmitId)
            .Select(group =>
            {
                var submit = group.First().Submit;
                var scoredReviews = group
                    .Where(p => p.JudgingFiledAnswereds is { Count: > 0 })
                    .ToList();
                var scores = scoredReviews
                    .SelectMany(p => p.JudgingFiledAnswereds)
                    .Select(p => p.Rate)
                    .ToList();
                var reviewCount = group.Count();
                var completedReviewCount = scoredReviews.Count;
                var average = scores.Count == 0 ? 0 : scores.Average();
                var meetsMinimum = completedReviewCount >= request.MinimumReviews;
                var recommendation = meetsMinimum && average >= 4
                    ? "Strong candidate"
                    : meetsMinimum && average >= 3
                        ? "Consider for review"
                        : "Needs more review";
                var explanation = scores.Count == 0
                    ? "No completed score is available yet."
                    : $"{completedReviewCount} of {reviewCount} assigned reviews have scores; " +
                      $"average score is {average:0.00}.";

                return new SelectionRecommendationResponse
                {
                    SubmitId = submit.Id,
                    ProjectId = submit.ProjectId,
                    ProjectTitle = submit.Project?.Title,
                    TrackingCode = submit.TrackingCode,
                    ProjectType = (ProjectType)submit.Project.ProjectType,
                    ReviewCount = reviewCount,
                    CompletedReviewCount = completedReviewCount,
                    AverageScore = average,
                    SubmittedOn = submit.SubmitDate,
                    JudgingStatus = (JudgingStatus)submit.JudgingStatus,
                    Recommendation = recommendation,
                    Explanation = explanation
                };
            })
            .Where(p => p.CompletedReviewCount >= request.MinimumReviews || p.ReviewCount > 0)
            .OrderByDescending(p => p.AverageScore)
            .ThenByDescending(p => p.CompletedReviewCount)
            .ThenBy(p => p.SubmittedOn)
            .Take(100)
            .ToList();

        return await Result<List<SelectionRecommendationResponse>>.SuccessAsync(recommendations);
    }
}
