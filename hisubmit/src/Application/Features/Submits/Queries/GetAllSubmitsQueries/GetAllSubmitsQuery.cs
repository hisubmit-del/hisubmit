using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Submits;
using HiSubmit.Domain.Entities.Submitter;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Services.Identity;
using Hisubmit.Client.SharedModels.Features.Submits.Queries.GetAllSubmitsQueries;

namespace HiSubmit.Application.Features.Submits.Queries.GetAllSubmitsQueries;

public class GetAllSubmitsQuery : GetAllSubmitsRequest, IRequest<PaginatedResult<GetAllSubmitsResponse>>;

public class GetAllSubmitsQueryHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    ICurrentUserService currentUserService,
    IUserService userService)
    : IRequestHandler<GetAllSubmitsQuery, PaginatedResult<GetAllSubmitsResponse>>
{
    private readonly IUserService _userService = userService;

    public async Task<PaginatedResult<GetAllSubmitsResponse>> Handle(GetAllSubmitsQuery request,
        CancellationToken cancellationToken)
    {
        request.UserId = request.GetCurrentUserSubmits ? currentUserService.UserId : request.UserId;
        var submits = await unitOfWork.Repository<Submit>()
            .Entities
            .Include(submit => submit.Project)
            .Specify(new GetAllSubmitsFilterSpecification(request))
            .ProjectTo<GetAllSubmitsResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
        return submits;
    }
}