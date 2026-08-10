using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Domain.Enums;
using MediatR;

namespace HiSubmit.Application.Events.Payments.DeleteFestivalSubmitItemInCartItem.Handlers;

public class DeleteSubmitItemHandler(IUnitOfWork<int> unitOfWork)
    : INotificationHandler<DeletedFestivalSubmitItemInCartItemEvent>
{
    public async Task Handle(DeletedFestivalSubmitItemInCartItemEvent notification, CancellationToken cancellationToken)
    {
        var submit = await unitOfWork.Repository<Submit>()
            .GetByIdAsync(notification.SubmitId);
        if (submit != null)
        {
            submit.SubmitStatus = SubmitStatus.Deleted;
            await unitOfWork.Repository<Submit>()
                .UpdateAsync(submit);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}       