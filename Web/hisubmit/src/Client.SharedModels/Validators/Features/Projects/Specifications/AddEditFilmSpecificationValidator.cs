using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditFilmSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditMusicSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditScriptSpecification;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditVrXrSpecification;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects.Specifications;

public class AddEditFilmSpecificationValidator:AbstractValidator<AddEditFilmSpecificationCommand>
{
    public AddEditFilmSpecificationValidator(IStringLocalizer<AddEditFilmSpecificationValidator> localize)
    {
        // RuleFor(p => p.FilmingCountryId)
        //     .NotNull()
        //     .NotEqual(0)
        //     .WithMessage(localize["select filming country"]);
        
        RuleFor(p => p.OriginCountryId)
            .NotNull()
            .NotEqual(0)
            .WithMessage(localize["select origin country"]);

    }
}

public class AddEditMusicSpecificationValidator : AbstractValidator<AddEditMusicSpecificationCommand>
{
    public AddEditMusicSpecificationValidator(IStringLocalizer<AddEditMusicSpecificationValidator> localize)
    {
        RuleFor(p => p.OriginCountryId)
            .NotEqual(0)
            .NotNull()
            .WithMessage(localize["select origin country"]);
    }
}

public class AddEditScriptSpecificationValidator : AbstractValidator<AddEditScriptSpecificationCommand>
{
    public AddEditScriptSpecificationValidator(IStringLocalizer<AddEditScriptSpecificationValidator> localize)
    {
        RuleFor(p => p.OriginCountryId)
            .NotEqual(0)
            .NotNull()
            .WithMessage(localize["select origin country"]);
    }
}

public class AddEditVrXrSpecificationValidator : AbstractValidator<AddEditVrXrSpecificationCommand>
{
    public AddEditVrXrSpecificationValidator(IStringLocalizer<AddEditVrXrSpecificationValidator> localize)
    {
        RuleFor(p=>p.OriginCountryId) 
            .NotEqual(0)
            .NotNull()
            .WithMessage(localize["select origin country"]);
    }
}

public class AddEditPhotographicSpecificationValidator : AbstractValidator<AddEditVrXrSpecificationCommand>
{
    public AddEditPhotographicSpecificationValidator(IStringLocalizer<AddEditPhotographicSpecificationValidator> localize)
    {
        RuleFor(p=>p.OriginCountryId) 
            .NotEqual(0)
            .NotNull()
            .WithMessage(localize["select origin country"]);
    }
}

