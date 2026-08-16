using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Events.Festivals.FestivalREquestedSpecials;
using HiSubmit.Application.Features.Wrapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.Festivals.Commands.SpecialRequest;

public class SpecialRequestCommand : IRequest<IResult>
{
    public int FestivalId { get; set; }
}

public class SpecialRequestCommandHandler : FeatureBaseService<SpecialRequestCommandHandler>,
    IRequestHandler<SpecialRequestCommand, IResult>
{
    private readonly IMediator _mediator;
    public SpecialRequestCommandHandler(IMapper mapper, IUnitOfWork<int> unitOfWork,
        IStringLocalizer<SpecialRequestCommandHandler> localize,IMediator mediator)
        : base(mapper, unitOfWork, localize)
    {
        _mediator = mediator;
    }

    public async Task<IResult> Handle(SpecialRequestCommand request, CancellationToken cancellationToken)
    {
        var festival = await _unitOfWork.Repository<Festival>()
            .GetByIdAsync(request.FestivalId);
        if (festival.FeeStatus == FeeStatus.Special)
        {
            return await Result.FailAsync(_localize["your festival is special"]);
        }

        festival.FeeStatus = FeeStatus.SpecialRequest;
        await _unitOfWork.Repository<Festival>().UpdateAsync(festival);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new FestivalRequestedSpecial { FestivalId = festival.Id }, cancellationToken);
        return await Result.SuccessAsync(_localize["Your request has been filed."]);
    }
}