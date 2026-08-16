using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetFilmSpecificationDetail;

namespace HiSubmit.Application.Features.Projects.Queries.GetFilmSpecificationDetail;

public class GetFilmSpecificationDetailQuery
    :GetFilmSpecificationDetailRequest, IRequest<Result<GetFilmSpecificationDetailResponse>>;

public class GetFilmSpecificationDetailQueryHandler(
    IUnitOfWork<int> unitOfWork,
    IMapper mapper,
    IStringLocalizer<GetFilmSpecificationDetailQueryHandler> localizer)
    : IRequestHandler<GetFilmSpecificationDetailQuery, Result<GetFilmSpecificationDetailResponse>>
{
    public async Task<Result<GetFilmSpecificationDetailResponse>> Handle(GetFilmSpecificationDetailQuery request, CancellationToken cancellationToken)
    {
        var specification = await unitOfWork.Repository<FilmSpecification>().Entities
            .Where(p => p.ProjectId == request.ProjectId)
            .Include(p => p.ProjectTypes)
            .ProjectTo<GetFilmSpecificationDetailResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (specification != null)
        {
            return await Result<GetFilmSpecificationDetailResponse>.SuccessAsync(specification);
        }

        var newSpec = new GetFilmSpecificationDetailResponse() { SubProjectTypeIds = new List<int>() };
        return await Result<GetFilmSpecificationDetailResponse>.SuccessAsync(newSpec);
    }
}