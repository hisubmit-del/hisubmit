using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Projects;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Projects.Commands.ReleaseProject;

public class ReleaseProjectCommand:IRequest<IResult>
{
    public int Id { get; set; }
}

public class ReleaseProjectCommandHandler : IRequestHandler<ReleaseProjectCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly ICheckPermission _checkPermission;
    private readonly IStringLocalizer<ReleaseProjectCommandHandler> _localizer;

    public ReleaseProjectCommandHandler(IUnitOfWork<int> unitOfWork, ICheckPermission checkPermission, IStringLocalizer<ReleaseProjectCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _checkPermission = checkPermission;
        _localizer = localizer;
    }

    public async Task<IResult> Handle(ReleaseProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _unitOfWork.Repository<Project>()
            .Entities.Include(p=>p.ProjectFiles)
            .FirstOrDefaultAsync(p=>p.Id==request.Id,cancellationToken);
        await _checkPermission.CheckWrightProjectPermission(project.UserId);

        var messages = new List<string>();
        if (! project.ProjectFiles.Any())
        {
            messages.Add(_localizer["The files section has not been completed "]);
        }
        if (messages.Any())
        {
            return await Result.FailAsync(messages);
        }

        return await Result.SuccessAsync(_localizer["Your project Successfully released"]);
    }
}