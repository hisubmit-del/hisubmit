using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Submits.Commands;

public class WithDrawProjectCommand:IRequest<IResult>
{
    public int Id { get; set; }
}

public class WithdrawProjectCommandHandler : IRequestHandler<WithDrawProjectCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<WithdrawProjectCommandHandler> _localizer;
    private readonly ICurrentUserService _currentUserService;

    public WithdrawProjectCommandHandler
        (IUnitOfWork<int> unitOfWork,
         IStringLocalizer<WithdrawProjectCommandHandler> localizer,
         ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _currentUserService = currentUserService;
    }
    public async Task<IResult> Handle(WithDrawProjectCommand request, CancellationToken cancellationToken)
    {
        if (!_currentUserService.IsAuthenticated)
            return await Result.FailAsync(_localizer["You must be signed in to withdraw a submission"]);

        var withDraw = await _unitOfWork.Repository<Submit>()
            .Entities
            .Include(submit => submit.Project)
            .FirstOrDefaultAsync(submit => submit.Id == request.Id, cancellationToken);

        if (withDraw != null)
        {
            var canWithdraw = _currentUserService.IsInRole(RoleConstants.AdministratorRole) ||
                              withDraw.Project.UserId == _currentUserService.UserId;

            if (!canWithdraw)
                return await Result.FailAsync(_localizer[
                    "Only the artist who owns the submission can withdraw it"]);

            withDraw.SubmitStatus = SubmitStatus.Withdrawn;
            await _unitOfWork.Repository<Submit>().UpdateAsync(withDraw);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(_localizer["The project was withdrawn"]);
        }
        else
        {
            return await Result.FailAsync(_localizer["An error has occurred"]);
        }
    }
}
