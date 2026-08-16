using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Payments.DiscountsCodes.Commands;

public class AddEditDiscountCodeCommand : AddEditDiscountCodeRequest, IRequest<IResult>;

internal class AddEditDiscountCommandHandler(IUnitOfWork<int> unitOfWork, IMapper mapper) 
    : IRequestHandler<AddEditDiscountCodeCommand, IResult>
{
    public async Task<IResult> Handle(AddEditDiscountCodeCommand request, CancellationToken cancellationToken)
    {
        request.Code = request.Code.Trim();
        if (await checkDuplicateCode(request.Code, request.FestivalId,request.Id))
            return await Result.FailAsync("The discount code entered is duplicate.");


        if (request.Id == 0)
        {
            var mappedCode = mapper.Map<DiscountCode>(request);
            await unitOfWork.Repository<DiscountCode>().AddAsync(mappedCode);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync("Discount code created successfully.");
        }

        var dbCode = await unitOfWork.Repository<DiscountCode>()
            .GetByIdAsync(request.Id);
        if (dbCode == null)
            return await Result.FailAsync("Operation Failed");

        var updatedCode = mapper.Map(request, dbCode);
        await unitOfWork.Repository<DiscountCode>().UpdateAsync(updatedCode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync("Discount code updated successfully.");
    }

    private async Task<bool> checkDuplicateCode(string code, int? festivalId, int id)
    {
        var existCode = await unitOfWork.Repository<DiscountCode>()
            .Entities
            .Where(p => p.Code == code.Trim()
                        && (p.FestivalId == null || p.FestivalId == festivalId)
                        &&(p.Id!=id))
            .AnyAsync();

        return existCode;
    }
}