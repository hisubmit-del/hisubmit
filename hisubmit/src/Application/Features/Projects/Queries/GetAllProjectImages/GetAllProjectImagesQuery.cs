using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Projects.Queries.GetAllProjectImages;

public class GetAllProjectImagesQuery:PagedRequest, IRequest<PaginatedResult<GetAllProjectImageResponse>>
{
    public int  ProjectId { get; set; }
}

public class GetAllProjectImageQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IStringLocalizer<GetAllProjectImageQueryHandler> localize)
    : IRequestHandler<GetAllProjectImagesQuery, PaginatedResult<GetAllProjectImageResponse>>
{
    private readonly IStringLocalizer<GetAllProjectImageQueryHandler> _localize = localize;

    public async Task<PaginatedResult<GetAllProjectImageResponse>> Handle(GetAllProjectImagesQuery request, CancellationToken cancellationToken)
    {
        var images = await unitOfWork.Repository<ProjectImage>()
            .Entities.Where(p => p.ProjectId == request.ProjectId)
            .ProjectTo<GetAllProjectImageResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);

        return images;
    }
}

public class GetAllProjectImageResponse
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string Location { get; set; }
    public string State { get; set; }
    public int ProjectId { get; set; }
}