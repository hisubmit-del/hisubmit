using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Specifications.Projects;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetScreenAward;

namespace HiSubmit.Application.Features.Projects.Queries.GetScreenAward;

public class GetScreenAwardQuery : GetScreenAwardRequest, IRequest<Result<List<GetScreenAwardResponse>>>;

internal class GetScreenAwardQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetScreenAwardQuery, Result<List<GetScreenAwardResponse>>>
{
    public async Task<Result<List<GetScreenAwardResponse>>> Handle(GetScreenAwardQuery request, CancellationToken cancellationToken)
    {
        var spec = new ScreenAwardSpecification(request.ProjectId);
        var awards = await unitOfWork.Repository<ScreeningAward>()
            .Entities
            .Specify(spec)
            .ProjectTo<GetScreenAwardResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<GetScreenAwardResponse>>.SuccessAsync(awards);

    }
}