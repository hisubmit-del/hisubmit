using System;
using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Application.Events.Submits;
using HiSubmit.Application.Features.SpecialAccounts.Queries;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Requests.AnswerQuestions;
using HiSubmit.Application.Interfaces.Services.Identity;

namespace HiSubmit.Application.Features.Submits.Commands;

public class AddSubmitCommand : IRequest<Result<int>>
{
    public int? ProjectId { get; set; }
    public int FestivalId { get; set; }
    public List<int> DeadlineEventCategoriesId { get; set; }
    public List<AnswerQuestionDto> SubmitAnswerQuestions { get; set; }
}

public class AddSubmitCommandHandler : IRequestHandler<AddSubmitCommand, Result<int>>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IStringLocalizer<AddSubmitCommandHandler> _localize;

    public AddSubmitCommandHandler(
        IMapper mapper, IUnitOfWork<int> unitOfWork,
        IUserService userService,
        IStringLocalizer<AddSubmitCommandHandler> localize,
        IMediator mediator,
        ICurrentUserService currentUserService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localize = localize;
        _userService = userService;
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task<Result<int>> Handle
        (AddSubmitCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork
            .Repository<Project>()
            .GetByIdAsync(request.ProjectId.Value);

        var userState = _mediator.Send(new GetUserAccountTypeQuery()
        {
            UserId = project.UserId
        }, cancellationToken).Result.Data;

        var feeStatus = userState.Id == 0 ? FeeStatus.Usual : FeeStatus.Special;

        var festival = await _unitOfWork.Repository<Festival>()
            .Entities
            .Where(p => p.Id == request.FestivalId)
            .FirstOrDefaultAsync(cancellationToken);


        var sumPrice = 0.0;


        var deadLineCategories = new List<SubmitDeadLineCategories>();
        foreach (var deadlineId in request.DeadlineEventCategoriesId)
        {
            var deadLineCategory = await _unitOfWork.Repository<DeadlineEventCategory>()
                .GetByIdAsync(deadlineId);

            var minPrice = GetMinPrice(deadLineCategory, project, feeStatus);
            if (minPrice.Value != null)
            {
                sumPrice += minPrice.Value.Value;

                deadLineCategories.Add(new SubmitDeadLineCategories()
                {
                    Price = minPrice.Value.Value,
                    FeeType = minPrice.Key,
                    DeadlineEventCategoryId = deadlineId
                });
            }
        }

        var submit = _mapper.Map<Submit>(request);
        submit.SubmitDate = DateTime.Now;
        submit.JudgingStatus = JudgingStatus.NotSelected;
        submit.SubmitDeadlineEventCategories = deadLineCategories;
        submit.TrackingCode = await CalculateTrackingCode(festival.Prefix, festival.StartingNumber, festival.Id);
        await _unitOfWork.Repository<Submit>().AddAsync(submit);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(new ProjectSubmitedEvent()
        {
            Price = sumPrice,
            SubmitId = submit.Id,
            ProjectName = project.Title,
            ImageUrl = festival.LogoURL,
            FestivalName = festival.Name,
            UserId = _currentUserService.UserId,
            Title = $"Submit to {festival.Name}",
            FeeStatus = feeStatus
        }, cancellationToken);

        return await Result<int>.SuccessAsync(submit.Id);
    }


    private KeyValuePair<FeeType, double?>
        GetMinPrice(DeadlineEventCategory deadLineCategory,
            Project project, FeeStatus feeStatus)
    {
        var prices = new Dictionary<FeeType, double?>();

        if (deadLineCategory.StandardFee != null)
            prices.Add(FeeType.Standard, deadLineCategory.StandardFee.Value);

        if (project.StudentProject && deadLineCategory.StudentFee != null)
            prices.Add(FeeType.Student, deadLineCategory.StudentFee.Value);

        if (feeStatus == FeeStatus.Special && deadLineCategory.GoldFee != null)
            prices.Add(FeeType.Gold, deadLineCategory.GoldFee.Value);


        var f = prices.FirstOrDefault
            (k => k.Value == prices.Values.Min());
        return f;
    }

    private async Task<string> CalculateTrackingCode(string festivalPrefix, int trackingStartingNumber, int festivalId)
    {
        var lastTrackCode = await _unitOfWork.Repository<Submit>()
            .Entities
            .Where(p => p.FestivalId == festivalId)
            .OrderBy(p=>p.Id)
            .LastOrDefaultAsync();

        if (lastTrackCode != null && !string.IsNullOrWhiteSpace(lastTrackCode.TrackingCode))
        {
            var number = int.Parse(lastTrackCode.TrackingCode.Replace(festivalPrefix, ""));
            return $"{festivalPrefix}{number + 1}";
        }

        return $"{festivalPrefix}{trackingStartingNumber}";
    }
}
