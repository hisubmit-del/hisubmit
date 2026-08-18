using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Client.SharedModels.Features.Seo;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Catalog;
using HiSubmit.Domain.Entities.SeoTags;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Products.Queries.GetById;

public class GetProductByIdQuery : GetProductByIdRequest, IRequest<IResult<AddEditProductRequest>>;


public class GetProductByIdQueryHandler(IMapper mapper,IUnitOfWork<int> unitOfWork):IRequestHandler<GetProductByIdQuery,IResult<AddEditProductRequest>>
{
    
    public async Task<IResult<AddEditProductRequest>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await unitOfWork.Repository<Product>()
            .Entities
            .Include(p=>p.ProductImages)
            .Where(p => p.Id == request.Id &&
                        (request.FestivalId == null || p.FestivalId == request.FestivalId))
            .ProjectTo<AddEditProductRequest>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (result is null)
            return await Result<AddEditProductRequest>.FailAsync("Product not found");

        var seoTag = await unitOfWork.Repository<MetaTag>()
            .Entities
            .Where(p => p.PageId == result.Id.ToString() && p.Type == PageType.Product)
            .ProjectTo<AddEditSeoTagRequest>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        result.SeoTag =seoTag;
        return await Result<AddEditProductRequest>.SuccessAsync(result);
    }
}

  
