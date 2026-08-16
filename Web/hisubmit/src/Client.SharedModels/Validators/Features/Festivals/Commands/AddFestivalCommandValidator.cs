using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class AddFestivalCommandValidator:AbstractValidator<AddFestivalCommand>
{
    public AddFestivalCommandValidator(IStringLocalizer<AddFestivalCommandValidator> localizer)
    {
        RuleFor(festival => festival.Name).NotNull().NotEmpty()
            .WithMessage(localizer["FestivalId name is required"]);
    }
}