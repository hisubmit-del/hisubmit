using FluentValidation;
using Hisubmit.Client.SharedModels.Features.SubUsers.Commands.AddExistingUserToFestival;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features;

public class AddExistingUserToFestivalCommandValidator:AbstractValidator<AddExistingUserToFestivalCommand>
{
    public AddExistingUserToFestivalCommandValidator(IStringLocalizer<AddExistingUserToFestivalCommand> localizer)
    {
        RuleFor(p => p.Email)
            .Must(email => !string.IsNullOrWhiteSpace(email)).WithMessage(localizer["Email is required"]);
        // .EmailAddress().WithMessage(localizer["The email address is invalid"]);

    }
}