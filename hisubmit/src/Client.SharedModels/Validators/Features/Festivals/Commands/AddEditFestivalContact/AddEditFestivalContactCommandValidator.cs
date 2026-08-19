using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact;
using Hisubmit.Client.SharedModels.Validators.Features.Locations.Commands.AddEdit;
using Microsoft.Extensions.Localization;
using Hisubmit.Client.SharedModels.Validators.Extensions;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands.AddEditFestivalContact;

public class AddEditFestivalContactCommandValidator:AbstractValidator<AddEditFestivalContactCommand>
{
    public AddEditFestivalContactCommandValidator(
        IStringLocalizer<AddEditFestivalContactCommandValidator> localize,
        IStringLocalizer<AddEditAddressCommandValidator> addressLocalize
        )
    {
        RuleFor(festival => festival.WebSite)
            .NotNull().NotEmpty()
            .WithMessage(localize["Website Could not be null"])
            .SiteUrl();
        RuleFor(festival => festival.Youtube)
            .YoutubeChannelName();
        RuleFor(festival => festival.Twitter)
            .TwitterUsername();
        RuleFor(festival => festival.Telegram)
            .TelegramUsername();
        RuleFor(festival => festival.Instagram)
            .InstagramUsername();
        RuleFor(festival => festival.WhatsAppNumber)
            .WhatsAppNumber();
        RuleFor(festival => festival.Facebook)
            .FacebookUsername();
        RuleFor(festival => festival.Email)
            .NotNull().NotEmpty()
            .WithMessage(localize["Email Could not be null"])
            .EmailAddress()
            .WithMessage(localize["Email Address is not valid"]);
            

        RuleFor(festival => festival.Address)
            .SetValidator(new AddEditAddressCommandValidator(addressLocalize));
        RuleFor(festival => festival.SubmissionAddress)               
            .SetValidator(new AddEditAddressCommandValidator(addressLocalize))
            .When(p=>p.SeparateSubmissiionAddress);            
    }
}
