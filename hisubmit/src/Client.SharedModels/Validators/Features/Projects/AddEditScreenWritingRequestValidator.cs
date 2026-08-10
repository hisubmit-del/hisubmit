using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects;

public class AddEditScreenWritingRequestValidator:AbstractValidator<AddEditScreenWritingRequest>
{
    public AddEditScreenWritingRequestValidator(IStringLocalizer<AddEditScreenWritingRequestValidator>localize)
    {
        RuleFor(p => p.CountryId)
            .NotEqual(0)
            .WithMessage(localize["select the country"]);

        RuleFor(p => p.Title)
            .NotNull()
            .NotEmpty()
            .WithMessage(localize["Title is required"]);
        
        RuleFor(p => p.City)
            .NotNull()
            .NotEmpty()
            .WithMessage(localize["City is required"]);
        
        RuleFor(p => p.Premiere)
            .NotNull()
            .NotEmpty()
            .WithMessage(localize["Premiere is required"]);
    }
    
    public  class UpdateScreenWritingCommandValidator:AbstractValidator<UpdateScreenWritingRequest>
    {
        public UpdateScreenWritingCommandValidator(IStringLocalizer<AddEditScreenWritingRequestValidator>localize)
        {
            RuleForEach(p => p.ScreenWritings)
                .SetValidator(new AddEditScreenWritingRequestValidator(localize));
        }
    }
}
