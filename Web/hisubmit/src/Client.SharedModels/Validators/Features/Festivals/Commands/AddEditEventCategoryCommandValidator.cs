using FluentValidation;
using Microsoft.Extensions.Localization;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditEventCategory;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class AddEditEventCategoryCommandValidator
    :AbstractValidator<AddEditEventCategoryCommand>
{
    public AddEditEventCategoryCommandValidator
    (IStringLocalizer<AddEditEventCategoryCommandValidator> localizer,
        IStringLocalizer<UpdateDeadlineCategoryFeeValidator> feeLocalizer)
    {
        RuleFor(p => p.Name)
            .NotNull().NotEmpty().WithMessage(localizer["Name is Required"]);

        RuleForEach(p => p.CategoryonFees)
            .SetValidator(new UpdateDeadlineCategoryFeeValidator(feeLocalizer));
    }
}
