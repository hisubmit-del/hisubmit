using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Users;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.AddFestival;

public class AddFestivalCommand:IRequest<Result<int>>
{
    public string UserId { get; set; }
    public  string Name { get; set; }
    public bool AddToCurrentUser { get; set; }
}

public class AddFestivalCommandHandler(
    IUnitOfWork<int> unitOfWork,
    ICurrentUserService currentUserService,
    IMediator mediator,
    IUserService userService,
    IStringLocalizer<AddFestivalCommand> localizer)
    : IRequestHandler<AddFestivalCommand, Result<int>>
{
    public async Task<Result<int>> Handle(AddFestivalCommand request, CancellationToken cancellationToken)
    {
        var festivalNameExist = await unitOfWork.Repository<Festival>()
            .Entities.AnyAsync(p => p.Name == request.Name, cancellationToken);

        if (festivalNameExist)
        {
            return await Result<int>.FailAsync(localizer["The name of the festival is repetitive"]);
        }

        if (!currentUserService.IsAuthenticated)
            return await Result<int>.FailAsync($"User not found");

        var userId = request.AddToCurrentUser ? currentUserService.UserId : request.UserId;

        if (string.IsNullOrWhiteSpace(userId))
            return await Result<int>.FailAsync($"User Id not found-{request.AddToCurrentUser}-{request.UserId}");

        if (currentUserService.IsInRole(RoleConstants.FestivalRole))
            return await Result<int>.FailAsync(
                localizer["A festival account already exists for this user."]);

        await mediator.Publish(new FestivalUserRegisteredEvent
            { FestivalName = request.Name, UserId = userId }, cancellationToken);

        var roleResult = await userService.AddToRoleAsync(userId, [RoleConstants.FestivalRole]);
        if (!roleResult.Succeeded)
            return await Result<int>.FailAsync(roleResult.Messages);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var festival = await unitOfWork.Repository<Festival>().Entities
            .Where(p => p.UserId == userId && p.Name == request.Name)
            .OrderByDescending(p => p.CreatedOn)
            .FirstOrDefaultAsync(cancellationToken);

        if (festival is null)
            return await Result<int>.FailAsync(localizer["Festival could not be created"]);

        return await Result<int>.SuccessAsync(festival.Id, localizer["Festival created"]);
    }
}
