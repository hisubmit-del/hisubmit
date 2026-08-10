using FluentValidation;
using Hisubmit.Client.SharedModels.Features.DistributionInformations.Commands;
using Hisubmit.Client.SharedModels.Features.Projects.Commands.UpdateScreenWritings;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Projects;

public class AddEditProjectDistributionItemValidator:AbstractValidator<AddEditDistributionInformationItemRequest>
{
    public AddEditProjectDistributionItemValidator(IStringLocalizer<AddEditProjectDistributionItemValidator>localize)
    {
        RuleFor(p => p.CountryId)
            .NotEqual(0)
            .WithMessage(localize["select the country"]);
    }
}

public class AddEditProjectDistributionValidator : AbstractValidator<AddEditDistributionInformationRequest>
{
    public AddEditProjectDistributionValidator(IStringLocalizer<AddEditProjectDistributionValidator> localize
    ,IStringLocalizer<AddEditProjectDistributionItemValidator>itemLocalize)
    {
        RuleFor(p => p.Title)
            .NotNull()
            .NotEmpty()
            .WithMessage(localize["Title is required"]);

        RuleForEach(p => p.Items)
            .SetValidator(new AddEditProjectDistributionItemValidator(itemLocalize));
    }
}

public class UpdateScreenWritingValidator : AbstractValidator<UpdateDistributionInformationCommand>
{
    public UpdateScreenWritingValidator(IStringLocalizer<UpdateScreenWritingValidator> localize,
        IStringLocalizer<AddEditProjectDistributionValidator>requestValidator,
        IStringLocalizer<AddEditProjectDistributionItemValidator> itemValidator)
    {
        RuleForEach(p => p.Information)
            .SetValidator(new AddEditProjectDistributionValidator(requestValidator,itemValidator));
    }
}

