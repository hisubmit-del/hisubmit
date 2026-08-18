using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using MediatR;

namespace HiSubmit.Application.Features.Payments.DiscountsCodes.Commands;

public class DeleteDiscountCodeCommand:BaseDeleteRequest, IRequest<IResult>
{
    public int? FestivalId { get; set; }
}

public class DeleteDiscountCodeCommandHandler(IUnitOfWork<int> unitOfWork):IRequestHandler<DeleteDiscountCodeCommand,IResult>
{
    public async Task<IResult> Handle(DeleteDiscountCodeCommand request, CancellationToken cancellationToken)
    {
        var delete = await unitOfWork.Repository<DiscountCode>()
            .GetByIdAsync(request.Id);
        if (delete == null)
            return await Result.FailAsync("Discount code not found.");

        if (delete.FestivalId != request.FestivalId)
            return await Result.FailAsync("You don't have permissions to this function ");
        await unitOfWork.Repository<DiscountCode>().DeleteAsync(delete);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("The discount code has been deleted");
    }
}
