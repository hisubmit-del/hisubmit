using FluentValidation;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Microsoft.Extensions.Localization;

namespace Hisubmit.Client.SharedModels.Validators.Features.Chats;

public class AddChatMessageCommandValidator:AbstractValidator<AddChatMessageRequest>
{
    public AddChatMessageCommandValidator(IStringLocalizer<AddChatMessageCommandValidator> localize)
    {
        RuleFor(p => p.Text).NotNull().NotEmpty()
            .WithMessage(localize["text is required"])
            .Must(p => p.Length < 200)
            .WithMessage(localize["text cant greater than 200 character"]);

        
        RuleFor(p => p.ChatRoomId).NotEqual(0)
            .WithMessage(localize["RoomId is required"]);
    }
}