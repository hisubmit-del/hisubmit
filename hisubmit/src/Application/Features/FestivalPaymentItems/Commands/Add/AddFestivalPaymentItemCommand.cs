using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using HiSubmit.Application.Interfaces.Repositories;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Domain.Enums.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using MediatR;
using Microsoft.Extensions.Localization;

namespace HiSubmit.Application.Features.FestivalPaymentItems.Commands.Add;

public class AddFestivalPaymentItemCommand : IRequest<IResult>
{
    public double Amount { get; set; }
    public int FestivalId { get; set; }
    public DateTime? PaidDate { get; set; }
    public string TrackNumber { get; set; }
    public FestivalPaymentType Type { get; set; }
}

public class AddFestivalPaymentItemCommandHandler : IRequestHandler<AddFestivalPaymentItemCommand, IResult>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddFestivalPaymentItemCommandHandler> _localizer;

    public AddFestivalPaymentItemCommandHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork, 
            IStringLocalizer<AddFestivalPaymentItemCommandHandler> localizer)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<IResult> Handle(AddFestivalPaymentItemCommand request, CancellationToken cancellationToken)
    {
        var item = _mapper.Map<FestivalPaymentItem>(request);
        await _unitOfWork.Repository<FestivalPaymentItem>().AddAsync(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return await Result.SuccessAsync(_localizer["Item added Successfully"]);
    }
}