using FluentValidation;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Requests.Identity
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator(IStringLocalizer<RoleRequestValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required"]);
        }
    }
}
