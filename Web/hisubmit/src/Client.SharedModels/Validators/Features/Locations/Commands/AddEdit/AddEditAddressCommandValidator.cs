using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Locatuions.Commands.AddEdit;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hisubmit.Client.SharedModels.Validators.Features.Locations.Commands.AddEdit
{
    public class AddEditAddressCommandValidator: AbstractValidator<AddEditAddressCommand>
    {
        public AddEditAddressCommandValidator(IStringLocalizer<AddEditAddressCommandValidator> stringLocalizer)
        {
            RuleFor(address => address.PostalCode).Must(postalCode => !string.IsNullOrWhiteSpace(postalCode))
                .WithMessage(stringLocalizer["PostalCode is Required"]);
            RuleFor(address => address.City).Must(city => !string.IsNullOrWhiteSpace(city))
                .WithMessage(stringLocalizer["City is Required"]);
            RuleFor(address => address.State).Must(state => !string.IsNullOrWhiteSpace(state))
                .WithMessage(stringLocalizer["State is Required"]);
            RuleFor(address => address.CountryId).Must(countryId => countryId !=0 )
                .WithMessage(stringLocalizer["Country is Required"]);
            RuleFor(address => address.Text).Must(text => !string.IsNullOrWhiteSpace(text))
                .WithMessage(stringLocalizer["Address is Required"]);
        }
    }
}
