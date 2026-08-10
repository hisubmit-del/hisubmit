using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Microsoft.Extensions.Localization;

namespace  Hisubmit.Client.SharedModels.Validators.Features.Reviews;

public class AddReviewCommandValidator : AbstractValidator<AddReviewCommand>
{
    public AddReviewCommandValidator(IStringLocalizer<AddReviewCommandValidator> localize)
    {
        // RuleFor(p => p.Text).NotNull().NotEmpty()
        //     .WithMessage(localize["The message is required"]);
        
        RuleFor(p => p.Text).Must(text=>text.Length>5)
            .When(p=>p.Text!=null)
            .WithMessage(localize["The message must be more than 5 characters"]);
    }
}

