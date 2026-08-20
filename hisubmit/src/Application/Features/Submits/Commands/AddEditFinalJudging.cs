using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Submits.Commands
{
    public class AddEditFinalJudgingCommand : IRequest<IResult>
    {
        public List<int> SubmitId { get; set; }
        public string Comment { get; set; }
        public JudgingStatus JudgingStatus { get; set; }
        public SubmitStatus SubmitStatus { get; set; }
    }

    public class AddEditFinalJudgingCommandHandler : IRequestHandler<AddEditFinalJudgingCommand, IResult>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<AddEditFinalJudgingCommandHandler> _localizer;
        private readonly ICurrentUserService _currentUserService;

        public AddEditFinalJudgingCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditFinalJudgingCommandHandler> localizer,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        public async Task<IResult> Handle(
            AddEditFinalJudgingCommand request,
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
                return await Result.FailAsync(
                    _localizer["You must be signed in to update a submission result"]);

            var submitIds = request.SubmitId?.Where(id => id > 0).Distinct().ToList() ?? [];
            if (submitIds.Count == 0)
                return await Result.FailAsync(_localizer["At least one submission is required"]);

            if (!IsValidResultCombination(request.JudgingStatus, request.SubmitStatus))
                return await Result.FailAsync(_localizer[
                    "The submission lifecycle status and judging result are incompatible"]);

            foreach (var submitId in submitIds)
            {
                var submit = await _unitOfWork.Repository<Submit>()
                    .Entities
                    .Include(item => item.Festival)
                        .ThenInclude(festival => festival.FestivalMaster)
                    .Include(item => item.Festival)
                        .ThenInclude(festival => festival.FestivalSubUsers)
                    .FirstOrDefaultAsync(item => item.Id == submitId, cancellationToken);

                if (submit is null)
                    return await Result.FailAsync(_localizer["Submit not found"]);

                var canUpdate = _currentUserService.IsInRole(RoleConstants.AdministratorRole) ||
                                submit.Festival.UserId == _currentUserService.UserId ||
                                submit.Festival.FestivalMaster?.UserId == _currentUserService.UserId ||
                                submit.Festival.FestivalSubUsers.Any(member =>
                                    member.UserId == _currentUserService.UserId &&
                                    !member.IsReferee &&
                                    !member.IsRemoved);

                if (!canUpdate)
                    return await Result.FailAsync(_localizer[
                        "Only the festival management team can update the final result"]);

                submit.JudgingStatus = request.JudgingStatus;
                submit.Comment = request.Comment;
                submit.SubmitStatus = request.SubmitStatus;
                await _unitOfWork.Repository<Submit>().UpdateAsync(submit);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(_localizer["submit status updated"]);
        }

        private static bool IsValidResultCombination(
            JudgingStatus judgingStatus,
            SubmitStatus submitStatus)
        {
            var positiveJudgingResult = judgingStatus is
                JudgingStatus.Selected or
                JudgingStatus.AwardWinner or
                JudgingStatus.Finalist or
                JudgingStatus.SemiFinalist or
                JudgingStatus.QuarterFinalist or
                JudgingStatus.Nominee or
                JudgingStatus.HonorableMention;

            if (!positiveJudgingResult)
                return true;

            return submitStatus is SubmitStatus.Paid or SubmitStatus.Inconsideration;
        }
    }
}
