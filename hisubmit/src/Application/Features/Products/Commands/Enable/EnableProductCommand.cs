using System;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Enums;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Products.Commands.Enable;

public class EnableProductCommand : IRequest<IResult>
{
    public int ProductId { get; set; }
    public bool IsEnable { get; set; }
    public ShowInSiteStatus Status { get; set; }
}

public class EnableProductCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<EnableProductCommandHandler> localizer)
    : IRequestHandler<EnableProductCommand, IResult>
{
    public async Task<IResult> Handle(EnableProductCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(request.ProductId);
        if (product == null)
            throw new NullReferenceException();

        product.IsEnable = request.IsEnable;
        // product.Status = request.Status;

        await unitOfWork.Repository<Product>().UpdateAsync(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["Product Updated"]);
    }
}
