using AutoMapper;
using AutoMapper.QueryableExtensions;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Specifications.Projects;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;
using System;
using System.Threading;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Projects.Queries.GetAll;

namespace HiSubmit.Application.Features.Projects.Queries.GetAll;

public class GetAllProjectQuery : GetAllProjectRequest, IRequest<PaginatedResult<GetAllProjectResponse>>;

public class GetAllProjectQueryHandler(
    IMapper mapper,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<GetAllProjectQueryHandler> localize,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetAllProjectQuery, PaginatedResult<GetAllProjectResponse>>
{
    private readonly IStringLocalizer<GetAllProjectQueryHandler> _localize = localize;

    public async Task<PaginatedResult<GetAllProjectResponse>> Handle(GetAllProjectQuery request, CancellationToken cancellationToken)
    {
        request.UserId = request.GetCurrentUserProjects ? currentUserService.UserId : request.UserId;
        var docSpec = new ProjectsFilterSpecification(request);

        var data = await unitOfWork.Repository<Project>().Entities
            .Specify(docSpec)
            .ProjectTo<GetAllProjectResponse>(mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request);
           
        return data;
    }
}