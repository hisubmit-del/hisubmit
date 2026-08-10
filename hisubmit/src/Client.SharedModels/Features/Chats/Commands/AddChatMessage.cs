using Hisubmit.Client.SharedModels.Enums.Chats;

namespace Hisubmit.Client.SharedModels.Features.Chats.Commands;

public class AddChatMessageRequest

{
    public string Text { get; set; }
    public string UserId { get; set; }
    public ChatMessageType Type { get; set; }
    public int ChatRoomId { get; set; }
}

