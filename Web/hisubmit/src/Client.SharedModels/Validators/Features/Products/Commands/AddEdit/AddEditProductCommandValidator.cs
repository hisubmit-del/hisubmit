using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Products.Commands.AddEdit;

public class AddEditProductCommandValidator : AbstractValidator<AddEditProductRequest>
{
    public AddEditProductCommandValidator(IStringLocalizer<AddEditProductCommandValidator> localize)
    {
        RuleFor(request => request.Name)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localize["Name is required!"]);
        // RuleFor(request => request.Barcode)
        //     .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Barcode is required!"]);
        RuleFor(request => request.Description)
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localize["Description is required!"]);
        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage(x => localize["Price must be greater than 0"]);
    }
}