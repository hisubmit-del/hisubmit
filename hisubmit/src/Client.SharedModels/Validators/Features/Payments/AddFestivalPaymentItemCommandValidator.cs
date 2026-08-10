using Hisubmit.Client.SharedModels.Enums.Festivals;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.FestivalPaymentItems.Commands.Add;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Payments;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Payments;

public class AddFestivalPaymentItemCommandValidator:AbstractValidator<AddFestivalPaymentItemCommand>
{
    public AddFestivalPaymentItemCommandValidator(IStringLocalizer<AddFestivalPaymentItemCommandValidator> localizer)
    {
        RuleFor(p => p.Amount)
            .GreaterThan(0)
            .WithMessage(localizer["Amount must be grater than zero "]);

        RuleFor(p => p.Type)
            .NotEqual(FestivalPaymentType.NotSelected)
            .WithMessage(localizer["Please Select type"]);

        RuleFor(p => p.PaidDate)
            .LessThanOrEqualTo(DateTime.Now)
            .WithMessage(localizer["The paid date cannot be later than the current date"]);

        RuleFor(p => p.TrackNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage(localizer["Tracking Number is Required"]);
    }
}

public class AddEditDiscountCodeRequestValidator : AbstractValidator<AddEditDiscountCodeRequest>
{
    public AddEditDiscountCodeRequestValidator()
    {
        RuleFor(p => p.CartItemTypes)
            .Must((disCode, itemType) => disCode.ForProducts || disCode.ForSubmissions || disCode.ForTickets)
            .WithMessage("Please select at least one sale type(Submissions,products,tickets) to apply the discount.");

        RuleFor(p => p.Code)
            .NotNull()
            .NotEmpty()
            .WithMessage("Code is Required");

        RuleFor(p => p.Count)
            .GreaterThan((short)0)
            .WithMessage("The discount code limit must be greater than zero");

    }
}