
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands.AddEditEventOrginizer
{
    public class AddEditEventOrginizerValidator:AbstractValidator<AddEditEventOrginizerCommand>
    {
        public AddEditEventOrginizerValidator(IStringLocalizer<AddEditEventOrginizerValidator> stringLocalizer)
        {
            RuleFor(orginizer => orginizer.Name)
                .NotNull().NotEmpty().WithMessage(stringLocalizer["Name is Required"]);
            RuleFor(orginizer => orginizer.Title)
                .NotNull().NotEmpty().WithMessage(stringLocalizer["Title is Required"]);
            RuleFor(orginizer => orginizer.FestivalId)
                           .NotEqual(0).WithMessage(stringLocalizer["FestivalId is Required"]);

        }
    }
}
