using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.AddEditTickets;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Tickets;

public class AddEditTicketCommandValidator:AbstractValidator<AddEditTicketsCommand>
{
    public AddEditTicketCommandValidator(IStringLocalizer<AddEditTicketCommandValidator> localizer)
    {
        RuleFor(p => p.VenueId).NotEqual(0)
            .WithMessage(localizer["Venue is required"]);
        RuleFor(p => p.Title)
            .NotNull().NotEmpty().WithMessage(localizer["Title is required"]);
        RuleFor(p => p.Capacity)
            .GreaterThan(0).WithMessage(localizer["The Capacity must greater than zero"]);
        RuleFor(p => p.Cost)
            .GreaterThanOrEqualTo(0).WithMessage(localizer["The Cost  cannot be negative"]);
    }
}