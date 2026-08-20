using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Enums;
using Hisubmit.Client.SharedModels.Features.Recommendations.Queries;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Projects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Recommendations.Queries;

public sealed class GetGoldFestivalRecommendationsQuery : GetGoldFestivalRecommendationsRequest,
    IRequest<Result<List<GoldFestivalRecommendation>>>;

public sealed class GetGoldFestivalRecommendationsQueryHandler
    : IRequestHandler<GetGoldFestivalRecommendationsQuery, Result<List<GoldFestivalRecommendation>>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetGoldFestivalRecommendationsQueryHandler(
        IUnitOfWork<int> unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<GoldFestivalRecommendation>>> Handle(
        GetGoldFestivalRecommendationsQuery request,
        CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated ||
            string.IsNullOrWhiteSpace(_currentUserService.UserId))
            return await Result<List<GoldFestivalRecommendation>>.FailAsync("You must be signed in.");

        var project = await _unitOfWork.Repository<Project>()
            .Entities
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId &&
                                      p.UserId == _currentUserService.UserId, cancellationToken);

        if (project is null)
            return await Result<List<GoldFestivalRecommendation>>.FailAsync(
                "You can only request recommendations for your own project.");

        var now = DateTime.Now;
        var festivals = await _unitOfWork.Repository<Festival>()
            .Entities
            .AsNoTracking()
            .Include(f => f.DeadLines)
            .Include(f => f.EventCategories)
                .ThenInclude(c => c.DeadlineEventCategories)
            .Where(f => f.IsActive && f.Public &&
                        f.DeadLines.Any(d => d.Date >= now))
            .ToListAsync(cancellationToken);

        var recommendations = festivals
            .Select(f => BuildRecommendation(f, project, now))
            .Where(r => r is not null)
            .OrderByDescending(r => r.MatchScore)
            .ThenBy(r => r.NextDeadline)
            .Take(12)
            .ToList();

        return await Result<List<GoldFestivalRecommendation>>.SuccessAsync(recommendations);
    }

    private static GoldFestivalRecommendation BuildRecommendation(
        Festival festival,
        Project project,
        DateTime now)
    {
        var nextDeadline = festival.DeadLines
            .Where(d => d.Date >= now)
            .OrderBy(d => d.Date)
            .FirstOrDefault();

        if (nextDeadline is null)
            return null;

        var matchingCategories = festival.EventCategories
            .Where(c => c.ProjectType is null || c.ProjectType == project.ProjectType)
            .ToList();

        var eventTypeMatches = festival.EventType == project.ProjectType ||
                               matchingCategories.Any(c => c.ProjectType == project.ProjectType);
        if (!eventTypeMatches)
            return null;

        var categoryMatch = matchingCategories.Any(c => c.ProjectType == project.ProjectType);
        var score = categoryMatch ? 100 : 70;
        var reasons = new List<string>
        {
            categoryMatch ? "matching category" : "matching opportunity type",
            $"deadline {nextDeadline.Date:yyyy-MM-dd}"
        };

        var hasGoldFee = festival.EventCategories
            .SelectMany(c => c.DeadlineEventCategories ?? new List<DeadlineEventCategory>())
            .Any(d => d.GoldFee.HasValue);

        if (hasGoldFee)
        {
            score += 5;
            reasons.Add("Gold fee available");
        }

        return new GoldFestivalRecommendation
        {
            FestivalId = festival.Id,
            FestivalName = festival.Name,
            FestivalUrl = festival.URL,
            EventType = (ProjectType)festival.EventType,
            NextDeadline = nextDeadline.Date,
            HasGoldFee = hasGoldFee,
            MatchScore = score,
            MatchReason = string.Join(" · ", reasons)
        };
    }
}
