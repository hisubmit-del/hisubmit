using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Features.ProjectJudgings.Queries.GetDetail;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
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

    public WithdrawProjectCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<WithdrawProjectCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<IResult> Handle(WithDrawProjectCommand request, CancellationToken cancellationToken)
    {
        var withDraw =await _unitOfWork.Repository<Submit>().GetByIdAsync(request.Id);
        if (withDraw != null)
        {
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