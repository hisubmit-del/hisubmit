using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using HiSubmit.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Seo.GetPAgeSeoTags;

public class GetPageSeoTagsQuery:IRequest<IResult<GetPageSeoTagResult>>
{
    public PageType  PageType { get; set; }
    public string PageId { get; set; }
}

public class GetPageSeoTagQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetPageSeoTagsQuery, IResult<GetPageSeoTagResult>>
{
    private readonly IMapper _mapper = mapper;
    private readonly IUnitOfWork<int> _unitOfWork = unitOfWork;
    
    public async Task<IResult<GetPageSeoTagResult>> Handle(GetPageSeoTagsQuery request, CancellationToken cancellationToken)
    {
        var res= await _unitOfWork.Repository<MetaTag>()
            .Entities
            .Where(p => p.PageId == request.PageId && p.Type == request.PageType)
            .ProjectTo<GetPageSeoTagResult>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        return await Result<GetPageSeoTagResult>.SuccessAsync(res);
    }
}

public class GetPageSeoTagResult:SeoTagDto
{
    
}
