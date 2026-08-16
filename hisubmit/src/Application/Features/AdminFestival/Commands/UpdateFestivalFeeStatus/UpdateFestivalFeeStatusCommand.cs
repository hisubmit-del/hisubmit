using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.Festivals.AdminAnsweredSpecialRequest;
using HiSubmit.Application.Interfaces.Repositories;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.AdminFestival.Commands.UpdateFestivalFeeStatus;

public class UpdateFestivalFeeStatusCommand :UpdateFestivalFeeStatusRequest, IRequest<IResult>;

public class UpdateFestivalFeeStatusCommandHandler : IRequestHandler<UpdateFestivalFeeStatusCommand, IResult>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<UpdateFestivalFeeStatusCommandHandler> _localize;


    public UpdateFestivalFeeStatusCommandHandler
    (IUnitOfWork<int> unitOfWork, IMediator mediator,
        IStringLocalizer<UpdateFestivalFeeStatusCommandHandler> localize)
    {
        _mediator = mediator;
        _localize = localize;
        _unitOfWork = unitOfWork;
    }

    public async Task<IResult> Handle(UpdateFestivalFeeStatusCommand request, CancellationToken cancellationToken)
    {
        var festival = await _unitOfWork.Repository<Festival>().GetByIdAsync(request.FestivalId);
        if (festival == null)
        {
            return await Result.FailAsync(_localize["Festival not found"]);
        }

        festival.FeeStatus =(FeeStatus) request.FeeStatus;
        await _unitOfWork.Repository<Festival>().UpdateAsync(festival);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new AdminAnsweredSpecialRequestEvent
        {
            FestivalId = festival.Id,
            FeeStatus =(FeeStatus) request.FeeStatus,
        }, cancellationToken); 
        return await Result.SuccessAsync(_localize["The festival has been updated"]);
    }
}
