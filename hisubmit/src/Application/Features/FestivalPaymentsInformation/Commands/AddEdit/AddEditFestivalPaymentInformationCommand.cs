using MediatR;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Domain.Enums.Festivals;
using Microsoft.Extensions.Localization;
using HiSubmit.Domain.Entities.Payments;
using HiSubmit.Application.Interfaces.Repositories;

namespace  HiSubmit.Application.Features.FestivalPaymentsInformation.Commands.AddEdit;

public class AddEditFestivalPaymentInformationCommand:IRequest<IResult>
{
    public  int Id { get; set; }
    public FestivalPaymentType Type { get; set; }
    [System.ComponentModel.DataAnnotations.EmailAddress]
    public string PaypalEmail { get; set; }
    public string CardNumber { get; set; }
    public string CVC { get; set; }
    public string Expires { get; set; }
    
    public  int FestivalId { get; set; }
}
public  class  AddEditFestivalPaymentInformationCommandHandler
    :IRequestHandler<AddEditFestivalPaymentInformationCommand,IResult>
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork<int> _unitOfWork;
    private readonly IStringLocalizer<AddEditFestivalPaymentInformationCommandHandler> _localizer;

    public AddEditFestivalPaymentInformationCommandHandler
        (IMapper mapper, IUnitOfWork<int> unitOfWork,
            IStringLocalizer<AddEditFestivalPaymentInformationCommandHandler> localizer)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _localizer = localizer;
    }
    public async Task<IResult> Handle
        (AddEditFestivalPaymentInformationCommand request, CancellationToken cancellationToken)
    {
        if (request.Type == FestivalPaymentType.Paypal &&
            string.IsNullOrWhiteSpace(request.PaypalEmail))
            return await Result.FailAsync(_localizer["A PayPal payout recipient email is required."]);

        if (request.Type == FestivalPaymentType.Paypal &&
            !new System.ComponentModel.DataAnnotations.EmailAddressAttribute()
                .IsValid(request.PaypalEmail))
            return await Result.FailAsync(_localizer["Enter a valid PayPal payout recipient email."]);

        if (request.Id==0)
        {
            var info = _mapper.Map<FestivalPaymentInformation>(request);
            await _unitOfWork.Repository<FestivalPaymentInformation>()
                .AddAsync(info);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            var dbInformation =await _unitOfWork.Repository<FestivalPaymentInformation>()
                .GetByIdAsync(request.Id);
            if (dbInformation == null || dbInformation.FestivalId != request.FestivalId)
                return await Result.FailAsync(_localizer["the information not found"]);
            var updatedInformation = _mapper.Map(request, dbInformation);
            updatedInformation = ClearUnusableField(updatedInformation);
            await _unitOfWork.Repository<FestivalPaymentInformation>()
                .UpdateAsync(updatedInformation); 
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return await Result.SuccessAsync(_localizer["Information saved successfully"]);
    }

    private  static  FestivalPaymentInformation ClearUnusableField(FestivalPaymentInformation info)
    {
        if (info.Type == FestivalPaymentType.Paypal)
        {
            info.CardNumber = string.Empty;
            info.Expires = string.Empty;
            info.CVC = string.Empty;
        }
        else
        {
            info.PaypalEmail = string.Empty;
        }

        return info;
    }
}
