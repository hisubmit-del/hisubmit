using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands;

public class AddEditFestivalAdditionalSettingValidator:AbstractValidator<AddEditAdditionalSettingCommand>
{
    public AddEditFestivalAdditionalSettingValidator(IStringLocalizer<AddEditFestivalAdditionalSettingValidator>localize)
    {
        RuleFor(p => p.MaximomLenght)
            .NotNull()
            .NotEqual(0)
            .WithMessage(localize["Maximum length could not be equal 0"])
            .When(p => !p.AllLenghtAccepted)
            .GreaterThanOrEqualTo(p => p.MinimomLenght)
            .WithMessage(localize["Minimum should be less than or equal Maximum"]);

        RuleFor(p => p.MinimomLenght)
            .NotNull()
            .WithMessage(localize["Maximum length could not be empty"])
            .When(p => !p.AllLenghtAccepted);

        RuleFor(p => p.URL)
            .NotNull().NotEmpty()
            .WithMessage("URl could not be empty");
        
        RuleFor(p => p.FestivalArtCategoriesId)
            .NotNull()
            .Must(p =>p is { Count: >= 1 })
            .WithMessage(localize["At least one category must be selected. "]);
        
        RuleFor(p => p.FestivalFestivalFociId)
            .NotNull()
            .Must(p =>p is { Count: >= 1 })
            .WithMessage(localize["At least one focus must be selected. "]);
        
        RuleFor(p => p.FestivalFestivalFociId)
            .NotNull()
            .Must(p =>p is { Count: <= 4 })
            .WithMessage(localize["The maximum allowable number for focus is four."]);
    }
}
