using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Features.Seo;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Domain.Entities.SeoTags;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.StaticPages.Queries;

public class GetDetailStaticPageQuery:IRequest<IResult<GetDetailStaticPageResponse>>
{
    public int Id { get; set; }
    public  string Link { get; set; }
    public  bool IsEnable { get; set; }
}

public class GetDetailStaticPageQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetDetailStaticPageQuery, IResult<GetDetailStaticPageResponse>>
{
    public async Task<IResult<GetDetailStaticPageResponse>> Handle(GetDetailStaticPageQuery request, CancellationToken cancellationToken)
    {
        StaticPageAndFAQ staticPageAndFaq;
        if (request.Id != 0)
        {
            staticPageAndFaq = await unitOfWork.Repository<StaticPageAndFAQ>().GetByIdAsync(request.Id);
        }
        else
        {
            var normalizedLink = request.Link?.Trim().Trim('/');
            staticPageAndFaq = await unitOfWork.Repository<StaticPageAndFAQ>()
                .Entities.Where(p => p.IsEnable == request.IsEnable &&
                                     p.Link != null &&
                                     (p.Link == normalizedLink || p.Link == "/" + normalizedLink))
                .FirstOrDefaultAsync(cancellationToken);
        }
        if (staticPageAndFaq == null) return await Result<GetDetailStaticPageResponse>
                .FailAsync("Static page not found");
        
        var mappedNew = mapper.Map<GetDetailStaticPageResponse>(staticPageAndFaq);
        mappedNew.SeoTag = await unitOfWork.Repository<MetaTag>()
            .Entities
            .Where(p => p.Type == PageType.StaticPage && p.PageId == mappedNew.Id.ToString())
            .ProjectTo<AddEditSeoTagRequest>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
            
        return await Result<GetDetailStaticPageResponse>.SuccessAsync(mappedNew);
    }
}

