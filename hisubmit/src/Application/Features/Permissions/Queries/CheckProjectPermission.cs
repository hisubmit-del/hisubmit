using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.Permissions.Queries;

public class CheckProjectPermissionQuery : IRequest<IResult<ProjectPermissionResponse>>
{
    public int ProjectId { get; set; }
}

public class
    CheckPermissionQueryHandler : IRequestHandler<CheckProjectPermissionQuery, IResult<ProjectPermissionResponse>>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public CheckPermissionQueryHandler(IUnitOfWork<int> unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<IResult<ProjectPermissionResponse>> Handle(CheckProjectPermissionQuery request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;
        if (string.IsNullOrWhiteSpace(currentUserId))
            return await Result<ProjectPermissionResponse>.SuccessAsync(ProjectPermissionResponse.Read);
        
        var projectUserId = await _unitOfWork.Repository<Project>()
            .Entities
            .Where(p => p.Id == request.ProjectId)
            .Select(p => p.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        var permission = projectUserId == currentUserId ? ProjectPermissionResponse.Write : ProjectPermissionResponse.Read;
        return await Result<ProjectPermissionResponse>.SuccessAsync(permission);
    }
}

public enum ProjectPermissionResponse
{
    Read = 0,
    Write = 1
}