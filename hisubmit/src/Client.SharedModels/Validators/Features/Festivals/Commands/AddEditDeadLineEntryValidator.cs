using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class AddEditDeadLineEntryValidator:AbstractValidator<AddEditDeadLineEntryRequest>
{
    public AddEditDeadLineEntryValidator(IStringLocalizer<AddEditDeadLineEntryValidator>localize)
    {
        RuleFor(deadLine => deadLine.Name).NotNull().NotEmpty().WithMessage(localize["Name is required"]);
        RuleFor(deadline => deadline.Date).NotNull().NotEmpty().WithMessage(localize["Date is required"]);
        RuleFor(deadLine => deadLine.ApplyToAllCategory).Equal(true)
            .When(deadLine => !deadLine.AddWithoutCategory &&(deadLine.CategoryId == null || deadLine.CategoryId.Count == 0 ))
            .WithMessage("Select Category Or select Apply To all Category");
    }
}

