using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Enums.Chats;

namespace HiSubmit.Domain.Entities.Chats;

public class ChatMessage : AuditableEntity<int>
{
    public string Text { get; set; }
    public string UserId { get; set; }
    public ChatMessageType Type { get; set; }
    public  bool Seen { get; set; }
    public ChatRoom ChatRoom { get; set; }
    public int ChatRoomId { get; set; }
}