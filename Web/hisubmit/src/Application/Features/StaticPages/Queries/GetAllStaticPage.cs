using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Contents;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Content;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.StaticPages.Queries;

public class GetAllStaticPageQuery : GetAllStaticPageRequest, IRequest<PaginatedResult<GetAllStaticPageResponse>>;

public class GetAllStaticPageQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<GetAllStaticPageQueryHandler> localizer,
    IMapper mapper)
    : IRequestHandler<GetAllStaticPageQuery, PaginatedResult<GetAllStaticPageResponse>>
{
    private readonly IStringLocalizer<GetAllStaticPageQueryHandler> _localizer = localizer;

    public async Task<PaginatedResult<GetAllStaticPageResponse>> Handle(GetAllStaticPageQuery request, CancellationToken cancellationToken)
    {
        var specification = new StaticPageAndFaqFilterSpecification(request);
           
        var response = await unitOfWork.Repository<StaticPageAndFAQ>()
            .Entities
            .Specify(specification)
            .ProjectTo<GetAllStaticPageResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return response;
    }
}
