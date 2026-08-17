using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace HiSubmit.Application.Features.Reviews.Queries;

public class GetFestivalRatingSummaryQuery : IRequest<Result<GetFestivalRatingSummaryResponse>>
{
    public int FestivalId { get; set; }
}

public class GetFestivalRatingSummaryResponse
{
    public double AverageRate { get; set; }
    public int TotalVotes { get; set; }
    public bool HasRated { get; set; }
}

public class GetFestivalRatingSummaryQueryHandler
    : IRequestHandler<GetFestivalRatingSummaryQuery, Result<GetFestivalRatingSummaryResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetFestivalRatingSummaryQueryHandler(
        IUnitOfWork<int> unitOfWork,
        ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result<GetFestivalRatingSummaryResponse>> Handle(
        GetFestivalRatingSummaryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.FestivalId <= 0)
            return await Result<GetFestivalRatingSummaryResponse>.FailAsync("Festival not found");

        var ratings = _unitOfWork.Repository<Review>()
            .Entities
            .Where(p => p.FestivalId == request.FestivalId && p.Type == CommentType.Review);

        var totalVotes = await ratings.CountAsync(cancellationToken);
        var averageRate = totalVotes == 0
            ? 0
            : await ratings.AverageAsync(p => (double)p.Rate, cancellationToken);

        var userId = _currentUserService.UserId;
        var clientIp = _currentUserService.UserIP;
        var hasRated = await ratings.AnyAsync(p =>
            (!string.IsNullOrWhiteSpace(userId) && p.UserId == userId) ||
            (!string.IsNullOrWhiteSpace(clientIp) && p.ClientIp == clientIp),
            cancellationToken);

        return await Result<GetFestivalRatingSummaryResponse>.SuccessAsync(
            new GetFestivalRatingSummaryResponse
            {
                AverageRate = Math.Round(averageRate, 1),
                TotalVotes = totalVotes,
                HasRated = hasRated
            });
    }
}
