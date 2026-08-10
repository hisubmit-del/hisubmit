using Hisubmit.Client.SharedModels.Enums.Festivals;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentsInformation.Commands.AddEdit;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Payments;

public class AddEditFestivalPaymentInformationCommandValidator
    : AbstractValidator<AddEditFestivalPaymentInformationCommand>
{
    public AddEditFestivalPaymentInformationCommandValidator
        (IStringLocalizer<AddEditFestivalPaymentInformationCommandValidator> localizer)
    {
        RuleFor(p => p.Type)
            .Must(p => p != FestivalPaymentType.NotSelected)
            .WithMessage("Please Select ItemType");
        
        RuleFor(p => p.PaypalEmail)
            .NotNull()
            .NotEmpty()
            .WithMessage("Email Address is Required")
            .EmailAddress()
            .WithMessage("Email Address not valid")
            .When(p => p.Type == FestivalPaymentType.Paypal);

        RuleFor(p => p.Expires)
            .NotNull()
            .NotEmpty()
            .WithMessage("Expires is Required")
            .When(p => p.Type == FestivalPaymentType.DebitCard);
        
        RuleFor(p => p.CVC)
            .NotNull()
            .NotEmpty()
            .WithMessage("CVC is Required")
            .When(p => p.Type == FestivalPaymentType.DebitCard);
        
        RuleFor(p => p.CardNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage("CardNumber is Required")
            .When(p => p.Type == FestivalPaymentType.DebitCard);

    }
}