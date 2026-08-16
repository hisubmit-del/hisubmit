using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.RemoveUserFromFestival;

public class RemoveUserFromFestivalCommand : IRequest<IResult>
{
    public int? Id { get; set; }
    public int? FestivalId { get; set; }
    public string UserId { get; set; }
}

public class RemoveUserFromFestivalCommandHandler
    : IRequestHandler<RemoveUserFromFestivalCommand, IResult>
{
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<RemoveUserFromFestivalCommandHandler> _localizer;

    public RemoveUserFromFestivalCommandHandler
        (IUnitOfWork<int> unitOfWork, IStringLocalizer<RemoveUserFromFestivalCommandHandler> localizer)
    {
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }

    public async Task<IResult> Handle(RemoveUserFromFestivalCommand request, CancellationToken cancellationToken)
    {
        FestivalSubUser festivalSubUser = null;
        if (request.Id != null)
            festivalSubUser = await _unitOfWork.Repository<FestivalSubUser>()
                .GetByIdAsync(request.Id.Value);
        else
        {
            if (request.FestivalId != null && !string.IsNullOrWhiteSpace(request.UserId))
            {
                festivalSubUser = await _unitOfWork.Repository<FestivalSubUser>()
                    .Entities
                    .Where(p => p.FestivalId == request.FestivalId.Value && p.UserId == request.UserId)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        if (festivalSubUser == null)
            return await Result.FailAsync(_localizer["Festival Or User Not Found"]);

        var refereeProjects = await _unitOfWork.Repository<ProjectJudging>()
            .Entities
            .Where(p => p.UserId == festivalSubUser.UserId
                        && p.Submit.FestivalId == festivalSubUser.FestivalId)
            .ToListAsync(cancellationToken);
        foreach (var rp in refereeProjects)
            rp.RefereeStatus = RefereeStatus.RemoveFromFestival;

        festivalSubUser.IsRemoved = true;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["Successfully removed from festival"]);
    }
}