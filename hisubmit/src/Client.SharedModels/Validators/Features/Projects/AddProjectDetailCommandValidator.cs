using Hisubmit.Client.SharedModels.Features.Projects.Commands.AddEditProjectDetail;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Hisubmit.Client.SharedModels.Validators.Extensions;


namespace Hisubmit.Client.SharedModels.Validators.Features.Projects;

public class AddEditProjectDetailCommandValidator : AbstractValidator<AddEditProjectDetailCommand>
{
    public AddEditProjectDetailCommandValidator(IStringLocalizer<AddEditProjectDetailCommandValidator> localize)
    {
        RuleFor(project => project.Title)
            .NotNull()
            .NotEmpty()
            .WithMessage("Title is required");
            
        
        RuleFor(project => project.ProjectType)
            .NotEmpty()
            .WithMessage("Select your project type");;

        RuleFor(p => p.WebSite)
            .SiteUrl();
        RuleFor(p => p.Twitter)
            .TwitterUsername();
        RuleFor(p => p.WhatsApp)
            .WhatsAppNumber();
        RuleFor(p => p.Telegram)
            .TelegramUsername();
        RuleFor(p => p.Instagram)
            .InstagramUsername();
        RuleFor(p => p.Youtube)
            .YoutubeChannelName();
    }
}