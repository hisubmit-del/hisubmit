using MediatR;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.ProductSold.AddProductSold;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Features.Locatuions.Commands.AddEdit;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.SoldProducts.Commands;

public class AddProductSoldCommand : IRequest<IResult>
{
    public string Email { get; set; }
    public int ProductId { get; set; }
    public ProductType ProductType { get; set; }
    public ProductSoldStatus Status { get; set; }
    public AddEditAddressCommand Address { get; set; }
}

public class AddSoldProductCommandHandler(
    IMapper mapper,
    ICurrentUserService currentUserService,
    IMediator mediator,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<AddSoldProductCommandHandler> localize)
    : IRequestHandler<AddProductSoldCommand, IResult>
{
    public async Task<IResult> Handle
        (AddProductSoldCommand request, CancellationToken cancellationToken)
    {
        var product = await unitOfWork.Repository<Product>()
            .GetByIdAsync(request.ProductId);
        var soldProduct = mapper.Map<ProductSold>(request);

        if (request.ProductType == ProductType.Downloadable)
        {
            soldProduct.AddressId = null;
            soldProduct.Address = null;
        }

        var siteCommission = await unitOfWork.Repository<SiteCommission>()
            .Entities.FirstOrDefaultAsync(cancellationToken);

        soldProduct.Status = ProductSoldStatus.AwaitingPayment;
        soldProduct.UserId = currentUserService.UserId;

        //soldProduct.Income = product.Price;

        // soldProduct.ShareFestivalIncome =(decimal) (1 - siteCommission.ProductSalesCommission) * soldProduct.Income;

        await unitOfWork.Repository<ProductSold>().AddAsync(soldProduct);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await mediator.Publish(new ProductSoldAddedEvent
        {
            Price = product.Price,
            ProductName = product.Name,
            ProductSoldId = soldProduct.Id,
            ProductImageUrl = product.ImageDataURL,
        }, cancellationToken);
        return await Result.SuccessAsync(localize["Product add to cart"]);
    }
}
