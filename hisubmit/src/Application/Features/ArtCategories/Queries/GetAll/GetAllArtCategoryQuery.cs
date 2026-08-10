using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Wrapper;
using LazyCache;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;

namespace HiSubmit.Application.Features.Brands.Queries.GetAll;

public class GetAllArtCategoryQuery :GetAllArtCategoryRequest, IRequest<Result<List<GetAllArtCategoryResponse>>>;

internal class
    GetAllBrandsCachedQueryHandler(IUnitOfWork<int> unitOfWork, IMapper mapper, IAppCache cache)
    : IRequestHandler<GetAllArtCategoryQuery, Result<List<GetAllArtCategoryResponse>>>
{
    public async Task<Result<List<GetAllArtCategoryResponse>>> Handle(GetAllArtCategoryQuery request,
        CancellationToken cancellationToken)
    {
        Func<Task<List<ArtCategory>>> getAllBrands = () => unitOfWork.Repository<ArtCategory>().GetAllAsync();
        var brandList = await cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllBrandsCacheKey, getAllBrands);
        var mappedBrands = mapper.Map<List<GetAllArtCategoryResponse>>(brandList);
        return await Result<List<GetAllArtCategoryResponse>>.SuccessAsync(mappedBrands);
    }
}
