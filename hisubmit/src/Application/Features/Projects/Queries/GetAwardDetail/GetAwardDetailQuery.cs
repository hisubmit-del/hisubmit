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
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAwardDetail;

namespace HiSubmit.Application.Features.Projects.Queries.GetAwardDetail;

public class GetAwardDetailQuery : GetAwardDetailRequest, IRequest<Result<List<GetAwardDetailResponse>>>;

public class GetAwardDetailQueryHandler(IMapper mapper, IUnitOfWork<int> unitOfWork)
    : IRequestHandler<GetAwardDetailQuery, Result<List<GetAwardDetailResponse>>>
{
    public async Task<Result<List<GetAwardDetailResponse>>> Handle(GetAwardDetailQuery request, CancellationToken cancellationToken)
    {
        var spec = new AwardFilterSpecification(request.ProjectId);
        var awards = await unitOfWork.Repository<Award>()
            .Entities
            .Specify(spec)
            .ProjectTo<GetAwardDetailResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return await Result<List<GetAwardDetailResponse>>.SuccessAsync(awards);

    }
}