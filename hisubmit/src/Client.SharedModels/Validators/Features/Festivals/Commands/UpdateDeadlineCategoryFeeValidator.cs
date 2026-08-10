using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class UpdateDeadlineCategoryFeeValidator
    :AbstractValidator<UpdateDeadlineCategoryonFee>
{
    public UpdateDeadlineCategoryFeeValidator
        (IStringLocalizer<UpdateDeadlineCategoryFeeValidator> localizer)
    {
        RuleFor(p => p.StandardFee)
            .NotNull().NotEmpty()
            .WithMessage(localizer["Standard fee cannot be empty"]);
    }
}