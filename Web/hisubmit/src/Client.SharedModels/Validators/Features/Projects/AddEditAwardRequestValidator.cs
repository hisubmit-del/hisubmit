using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditAward;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects;

public class AddEditAwardRequestValidator:AbstractValidator<AddEditAwardRequest>
{
    public AddEditAwardRequestValidator()
    {
        RuleFor(p => p.Date)
            .NotNull().NotEmpty()
            .WithMessage("Date is required");
        RuleFor(p => p.Title)
            .NotNull().NotEmpty()
            .WithMessage("Title is required");
        RuleFor(p => p.Location)
            .NotNull().NotEmpty()
            .WithMessage("Location is required");
        RuleFor(p => p.AwardsWon)
            .NotNull().NotEmpty()
            .WithMessage("Awards won is required");
    }
    
    public class  UpdateAwardCommandValidator:AbstractValidator<UpdateAwardRequest>
    {
        public UpdateAwardCommandValidator()
        {
            RuleForEach(p => p.Awards)
                .SetValidator(new AddEditAwardRequestValidator());
        }
    }
}
