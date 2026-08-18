using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;

namespace HiSubmit.Application.Features.Products.Commands.Delete
{
    public class DeleteProductCommand : IRequest<Result<int>>
    {
        public int Id { get; set; }
        public int FestivalId { get; set; }
    }

    internal class DeleteProductCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IStringLocalizer<DeleteProductCommandHandler> localizer)
        : IRequestHandler<DeleteProductCommand, Result<int>>
    {
        public async Task<Result<int>> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var product = await unitOfWork.Repository<Product>()
                .Entities
                .FirstOrDefaultAsync(p => p.Id == command.Id &&
                                          (command.FestivalId == 0 || p.FestivalId == command.FestivalId),
                    cancellationToken);
            if (product != null)
            {
                await unitOfWork.Repository<Product>().DeleteAsync(product);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return await Result<int>.SuccessAsync(product.Id, localizer["Product Deleted"]);
            }
            else
            {
                return await Result<int>.FailAsync(localizer["Product Not Found!"]);
            }
        }
    }
}
