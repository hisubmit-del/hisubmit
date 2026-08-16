using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Brands.Commands.Delete;

namespace HiSubmit.Application.Features.Brands.Commands.Delete;

public class DeleteBrandCommand :DeleteBrandRequest, IRequest<Result<int>>;

internal class DeleteBrandCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<DeleteBrandCommandHandler> localizer)
    : IRequestHandler<DeleteBrandCommand, Result<int>>
{
    public async Task<Result<int>> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
    {
        var brand = await unitOfWork.Repository<ArtCategory>()
            .GetByIdAsync(command.Id);
        if (brand !=null)
        {
                await unitOfWork.Repository<ArtCategory>().DeleteAsync(brand);
                await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, localizer["ArtCategory Deleted"]);
        }
        return await Result<int>.FailAsync(localizer["ArtCategory Not Found!"]);
    }
}