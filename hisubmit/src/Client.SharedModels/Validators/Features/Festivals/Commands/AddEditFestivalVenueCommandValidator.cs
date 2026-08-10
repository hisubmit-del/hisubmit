using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class AddEditFestivalVenueCommandValidator:AbstractValidator<AddEditFestivalVenueCommand>
{
    public AddEditFestivalVenueCommandValidator(IStringLocalizer<AddEditFestivalVenueCommandValidator> localizer)
    {
        RuleFor(venue => venue.Address.CountryId).NotEqual(0)
            .WithMessage(localizer["Country Not selected"]);
        RuleFor(venue => venue.Name).Must(name => !string.IsNullOrWhiteSpace(name))
            .WithMessage(localizer["The Name is Required"]);
    }
}