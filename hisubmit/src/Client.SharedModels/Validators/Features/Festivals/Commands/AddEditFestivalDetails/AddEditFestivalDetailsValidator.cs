using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands.AddEditFestivalDetails
{
    public class AddEditFestivalDetailsValidator:AbstractValidator<AddEditFestivalDetailCommand>
    {
        public AddEditFestivalDetailsValidator(IStringLocalizer<AddEditFestivalDetailsValidator> stringLocalizer)
        {
            RuleFor(festival => festival.Name)
                .NotNull().NotEmpty().WithMessage(stringLocalizer["Event Name is required"]);
            RuleFor(festival => festival.Description)
                .NotNull().NotEmpty().WithMessage(stringLocalizer["Event Description is required"]);
            RuleFor(festival => festival.Rules)
                .NotNull().NotEmpty().WithMessage(stringLocalizer["Rules  is required"]);
            RuleFor(festival => festival.FilmFestival)
                .Must((festival, filmFestival) => CannotBothSelectedFilmFestivalAndOnlineFestival(festival))
                .WithMessage(stringLocalizer["Film FestivalId and Online FestivalId Cannot both Selected"]);
        }

        private bool CannotBothSelectedFilmFestivalAndOnlineFestival(AddEditFestivalDetailCommand festival)
        {
            return !(festival.FilmFestival & festival.OnlineFestival);
        }
    }
}
