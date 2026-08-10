using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Submits.Commands;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hisubmit.Client.SharedModels.Validators.Submits;

public class AddSubmitCommandValidator:AbstractValidator<AddSubmitCommand>
{
    public AddSubmitCommandValidator(IStringLocalizer<AddSubmitCommandValidator> localize)
    {
        RuleFor(submit => submit.ProjectId).NotNull().NotEqual(0).WithMessage(localize["Project not selected"]);
        RuleFor(submit => submit.DeadlineEventCategoriesId).Must((deadCategoriesId) => deadCategoriesId.Count > 0)
            .WithMessage(localize["At least one item must be selected"]);
    }
}