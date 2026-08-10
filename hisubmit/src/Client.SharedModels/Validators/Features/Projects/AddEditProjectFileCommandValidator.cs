using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectFileURL;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects
{
    public class AddEditProjectFileCommandValidator : AbstractValidator<AddEditProjectFileURLRequest>
    {
        public AddEditProjectFileCommandValidator(IStringLocalizer<AddEditProjectFileCommandValidator> localize)
        {
            RuleFor(file => file.Name).NotNull().NotEmpty()
                .WithMessage(localize["Name is required"]);

            RuleFor(file => file.FileURl).NotEmpty()
                .When(p => p.IsLocalFile == false)
                .WithMessage("File Url is required");
          
        }
    }

}
