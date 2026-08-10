using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Submits.PaidSubmit.Handler;

public class ChangedSubmitEventHandler:INotificationHandler<PaidSubmitEvent>
{
    private readonly IUnitOfWork<int> _unitOfWork;

    public ChangedSubmitEventHandler(IUnitOfWork<int> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(PaidSubmitEvent notification, CancellationToken cancellationToken)
    {
        var submit = await _unitOfWork.Repository<Submit>()
            .GetByIdAsync(notification.SubmitId);
        if (submit != null)
        {
            submit.SubmitStatus = SubmitStatus.Inconsideration;
            await _unitOfWork.Repository<Submit>().UpdateAsync(submit);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
