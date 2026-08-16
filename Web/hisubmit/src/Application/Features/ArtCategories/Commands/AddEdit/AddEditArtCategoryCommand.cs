using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Domain.Entities.Catalog;
using Microsoft.Extensions.Localization;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Features.Brands.Commands.AddEdit;

namespace HiSubmit.Application.Features.Brands.Commands.AddEdit;

public  class AddEditArtCategoryCommand :AddEditArtCatgoryRequest, IRequest<Result<int>>;

internal class AddEditArtCategoryCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IStringLocalizer<AddEditArtCategoryCommandHandler> localizer)
    : IRequestHandler<AddEditArtCategoryCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddEditArtCategoryCommand command, CancellationToken cancellationToken)
    {
        if (command.Id == 0)
        {
            var brand = mapper.Map<ArtCategory>(command);
            await unitOfWork.Repository<ArtCategory>().AddAsync(brand);
            await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
            return await Result<int>.SuccessAsync(brand.Id, localizer["ArtCategory Saved"]);
        }
        else
        {
            var brand = await unitOfWork.Repository<ArtCategory>().GetByIdAsync(command.Id);
            if (brand != null)
            {
                brand.Name = command.Name ?? brand.Name;
                brand.Description = command.Description ?? brand.Description;
                await unitOfWork.Repository<ArtCategory>().UpdateAsync(brand);
                await unitOfWork.CommitAndRemoveCache(cancellationToken, ApplicationConstants.Cache.GetAllBrandsCacheKey);
                return await Result<int>.SuccessAsync(brand.Id, localizer["ArtCategory Updated"]);
            }
            return await Result<int>.FailAsync(localizer["ArtCategory Not Found!"]);
        }
    }
}
