using System.Linq;
using System;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.ViolationReportField;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Enums;
using HiSubmit.Domain.Entities.Submitter;
using Microsoft.EntityFrameworkCore;
using HiSubmit.Client.SharedModels.Constants.Role;

namespace HiSubmit.Application.Features.Reviews.Commands;

public class AddReviewCommand : IRequest<IResult>
{
    public string Text { get; set; }
    public string UserId { get; set; }
   
    public int Rate { get; set; }
    public CommentType Type { get; set; }
    public int FestivalId { get; set; }
}

public class AddReviewCommandHandler : IRequestHandler<AddReviewCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddReviewCommandHandler> _localize;
    private readonly ICurrentUserService _currentUserService;

    public AddReviewCommandHandler
    (
        IMapper mapper,
        IMediator mediator,
        IUnitOfWork<int> unitOfWork,
        ICurrentUserService currentUserService,
        IStringLocalizer<AddReviewCommandHandler> localize)
    {
        _mapper = mapper;
        _localize = localize;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(AddReviewCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            return await Result.FailAsync(_localize["You must be logged in"]);

        if (request.FestivalId <= 0)
            return await Result.FailAsync(_localize["Festival not found"]);

        if (request.Type == CommentType.Review)
        {
            if (request.Rate is < 1 or > 5)
                return await Result.FailAsync(_localize["Please select a rating between 1 and 5"]);

            var festival = await _unitOfWork.Repository<Festival>()
                .Entities
                .Where(p => p.Id == request.FestivalId)
                .Select(p => new { p.Id, p.EventEndDate })
                .FirstOrDefaultAsync(cancellationToken);

            if (festival == null)
                return await Result.FailAsync(_localize["Festival not found"]);

            if (!festival.EventEndDate.HasValue ||
                DateTime.Now < festival.EventEndDate.Value.AddDays(14))
                return await Result.FailAsync(
                    _localize["Reviews become available two weeks after the festival ends"]);

            var acceptedStatuses = new[]
            {
                JudgingStatus.Selected,
                JudgingStatus.AwardWinner,
                JudgingStatus.Finalist,
                JudgingStatus.SemiFinalist,
                JudgingStatus.QuarterFinalist,
                JudgingStatus.Nominee,
                JudgingStatus.HonorableMention
            };

            var hasAcceptedSubmission = await _unitOfWork.Repository<Submit>()
                .Entities
                .AnyAsync(p =>
                    p.FestivalId == request.FestivalId &&
                    p.Project.UserId == _currentUserService.UserId &&
                    p.SubmitStatus != SubmitStatus.DontPaid &&
                    p.SubmitStatus != SubmitStatus.Disqualified &&
                    p.SubmitStatus != SubmitStatus.Withdrawn &&
                    acceptedStatuses.Contains(p.JudgingStatus),
                    cancellationToken);

            if (!hasAcceptedSubmission)
                return await Result.FailAsync(
                    _localize["Only participants with an accepted submission can review this festival"]);

            var alreadyReviewed = await _unitOfWork.Repository<Review>()
                .Entities
                .AnyAsync(p => p.FestivalId == request.FestivalId &&
                               p.UserId == _currentUserService.UserId &&
                               p.Type == CommentType.Review,
                    cancellationToken);
            if (alreadyReviewed)
                return await Result.FailAsync(_localize["You have already registered your comment"]);
        }

        var model = _mapper.Map<Review>(request);
        model.UserId = _currentUserService.UserId;
        model.ClientIp = _currentUserService.UserIP;
        await _unitOfWork.Repository<Review>().AddAsync(model);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        var message = request.Type == CommentType.Review 
            ? "Review Send" : "Report Send";

        if (request.Type == CommentType.ViolationReport)
            await _mediator.Publish(new ViolationReportFieldEvent
            {
                UserId = model.UserId,
                FestivalId = request.FestivalId,
            }, cancellationToken);
        return await Result.SuccessAsync(_localize[message]);
    }
}
