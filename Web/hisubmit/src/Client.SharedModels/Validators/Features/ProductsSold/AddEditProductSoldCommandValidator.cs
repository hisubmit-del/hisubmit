using Hisubmit.Client.SharedModels.Enums;
using FluentValidation;
using Hisubmit.Client.SharedModels.Features.SoldProducts.Commands;
using Hisubmit.Client.SharedModels.Validators.Features.Locations.Commands.AddEdit;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.ProductsSold;

public class AddEditProductSoldCommandValidator:AbstractValidator<AddProductSoldCommand>
{
    public AddEditProductSoldCommandValidator
        (IStringLocalizer<AddEditProductSoldCommandValidator> localize,
            IStringLocalizer<AddEditAddressCommandValidator> addressValidatorLocalize)
    {
        RuleFor(productSold => productSold.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage(localize["Email is required"])
            .EmailAddress()
            .WithMessage(localize["The email address is invalid "])
            .When(productsSold => productsSold.ProductType == ProductType.Downloadable);

        RuleFor(productSold => productSold.Address)
            .SetValidator(new AddEditAddressCommandValidator(addressValidatorLocalize))
            .When(productSold=>productSold.ProductType==ProductType.Sent);
    }
}