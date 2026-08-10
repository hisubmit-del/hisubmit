using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hisubmit.Client.SharedModels.Validators.Features.Festivals.Commands.AddEditFestivalDeadLine;

public class AddEditFestivalDeadLineValidator : AbstractValidator<AddEditFestivalDeadlineCommand>
{
    public AddEditFestivalDeadLineValidator(IStringLocalizer<AddEditFestivalDeadLineValidator> localize)
    {
        RuleFor(p => p.OpeningDate).NotNull()
            .WithMessage(localize["Opening date is required"]);
        RuleFor(p => p.NotificationDate).NotNull()
            .WithMessage(localize["Notification Date is required"]);
        RuleFor(p => p.EventStartDate).NotNull()
            .WithMessage(localize["Event Start Date is required"]);
        RuleFor(p => p.EventEndDate).NotNull()
            .WithMessage(localize["Event End Date is required"]);

        RuleFor(p => p.NotificationDate)
            .Must((deadline,notificationDate) => deadline.OpeningDate < notificationDate)
            .When(p=>p.NotificationDate!=null && p.OpeningDate!=null)
            .WithMessage(localize["Notification date must be after opening date "]);
            
        RuleFor(p => p.EventStartDate)
            .Must((deadline,eventStartDate) => deadline.NotificationDate <eventStartDate)
            .When(p=>p.EventStartDate!=null && p.NotificationDate!=null)
            .WithMessage(localize["Event start  date must be after notification date "]);
            
        RuleFor(p => p.EventEndDate)
            .Must((deadline,eventEndDate) => deadline.EventStartDate<eventEndDate)
            .When(p=>p.EventStartDate!=null && p.EventEndDate!=null)
            .WithMessage(localize["Event end date must be after event start date "]);
    }
}