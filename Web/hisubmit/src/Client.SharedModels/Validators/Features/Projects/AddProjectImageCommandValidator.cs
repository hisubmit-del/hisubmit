using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.ProjectImages;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects;

public class AddProjectImageCommandValidator:AbstractValidator<AddProjectImageCommand>
{
    public AddProjectImageCommandValidator(IStringLocalizer<AddProjectImageCommandValidator> localize)
    {
        RuleFor(p => p.Title).NotNull().NotEmpty()
            .WithMessage(localize["title is required"]);
    }
}