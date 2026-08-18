using MediatR;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Application.Interfaces.Services.Identity;
using Microsoft.EntityFrameworkCore;

namespace HiSubmit.Application.Features.SubUsers.Commands.AddExistingUserToFestival;

public class AddExistingUserToFestivalCommand : IRequest<IResult>
{
    public string Email { get; set; }
    public  int FestivalId { get; set; }
}

public class AddExistingUserToFestivalCommandHandler : IRequestHandler<AddExistingUserToFestivalCommand, IResult>
{
    private readonly IUserService _userService;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddExistingUserToFestivalCommandHandler> _localizer;
    private readonly ICurrentUserService _currentUserService;

    public AddExistingUserToFestivalCommandHandler
    (IUserService userService, IUnitOfWork<int> unitOfWork,
        IStringLocalizer<AddExistingUserToFestivalCommandHandler> localizer,
        ICurrentUserService currentUserService)
    {
        _userService = userService;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
        _currentUserService = currentUserService;
    }

    public async Task<IResult> Handle(AddExistingUserToFestivalCommand request, CancellationToken cancellationToken)
    {
        var userid = await _userService.GetUserByEmailAddress(request.Email);
        if (string.IsNullOrWhiteSpace(userid))
            return await Result.FailAsync(_localizer["User not registered to the site "]);

        var existingMembership = await _unitOfWork.Repository<FestivalSubUser>()
            .Entities
            .FirstOrDefaultAsync(
                membership => membership.FestivalId == request.FestivalId &&
                              membership.UserId == userid,
                cancellationToken);

        if (existingMembership is not null)
        {
            if (!existingMembership.IsRemoved)
                return await Result.FailAsync(_localizer["User is already a member of this festival"]);

            existingMembership.IsRemoved = false;
            await _unitOfWork.Repository<FestivalSubUser>().UpdateAsync(existingMembership);
        }
        else
        {
            await _unitOfWork.Repository<FestivalSubUser>().AddAsync(new FestivalSubUser
            {
                UserId = userid,
                FestivalId = request.FestivalId
            });
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["Successfully added to festival"]);
    }
}
