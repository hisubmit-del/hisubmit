using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Payments.DiscountsCodes.Commands;

public class ChangeDiscountCodeStatusQuery : ChangeDiscountCodeStatusRequest, IRequest<IResult>;

internal class ChangeDiscountCodeStatusQueryHandler(IUnitOfWork<int> unitOfWork)
    :IRequestHandler<ChangeDiscountCodeStatusQuery,IResult>
{
    public async Task<IResult> Handle(ChangeDiscountCodeStatusQuery request, CancellationToken cancellationToken)
    {
        var discountCode = await unitOfWork.Repository<DiscountCode>()
            .Entities
            .Where(p => p.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (discountCode == null)
            return await Result.FailAsync("Operation Failed");

        if (discountCode.FestivalId != request.FestivalId)
            return await Result.FailAsync("You don't have permissions to this function ");

        discountCode.Enable = request.Enable;
        await unitOfWork.Repository<DiscountCode>().UpdateAsync(discountCode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Discount Code Status Updated");
    }
}
