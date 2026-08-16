using MediatR;
using System.Threading;
using HiSubmit.Domain.Enums;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Interfaces.Repositories;

namespace HiSubmit.Application.Features.Festivals.Commands.RemovedUserFromProject;

public class RemovedUserFromProjectCommand : IRequest<IResult>
{
    public int Id { get; set; }
}

public class RemovedUserFromProjectCommandHandler(
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<RemovedUserFromProjectCommandHandler> localizer) : IRequestHandler<RemovedUserFromProjectCommand, IResult>
{
    public async Task<IResult> Handle(RemovedUserFromProjectCommand request, CancellationToken cancellationToken)
    {
        var projectJudging = await unitOfWork.Repository<ProjectJudging>()
            .GetByIdAsync(request.Id);

        projectJudging.RefereeStatus = RefereeStatus.RemoveFromProject;
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(localizer["Successfully removed judging"]);
    }
}