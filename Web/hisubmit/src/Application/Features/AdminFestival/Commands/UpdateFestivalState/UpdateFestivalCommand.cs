using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using MediatR;
using Microsoft.Extensions.Localization;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Application.Events.Festivals.AdminAnsweredReleasedRequest;
using HiSubmit.Client.SharedModels.Constants.Application;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalState;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Enums;

namespace HiSubmit.Application.Features.AdminFestival.Commands.UpdateFestivalState;

public class UpdateFestivalStateCommand:UpdateFestivalStateRequest, IRequest<Result<int>>;
public class UpdateFestivalStateCommandHandler(
    IMediator mediator,
    IUnitOfWork<int> unitOfWork,
    IStringLocalizer<UpdateFestivalStateCommandHandler> localize)
    : IRequestHandler<UpdateFestivalStateCommand, Result<int>>
{
    public async Task<Result<int>> Handle(UpdateFestivalStateCommand request, CancellationToken cancellationToken)
    {
        var festival =await unitOfWork.Repository<Festival>().GetByIdAsync(request.Id);
        if(festival != null)
        {
            festival.IsActive = request.IsActive;
            festival.FestivalStatus = festival.IsActive ? FestivalStatus.Confirmed : FestivalStatus.NotConfirmed;
            await unitOfWork.Repository<Festival>().UpdateAsync(festival);
            await unitOfWork.CommitAndRemoveCache(cancellationToken,ApplicationConstants.Cache.GetAllFestivalCacheKey);
            await mediator.Publish(new AdminAnsweredReleasedRequestEvent
            {
                FestivalId = festival.Id,
                IsEnable = festival.IsActive
            },cancellationToken);
            return await Result<int>.SuccessAsync(festival.Id, localize["festival updated"]);
        }
        return await Result<int>.FailAsync(localize["Festival not found"]);
    }
}