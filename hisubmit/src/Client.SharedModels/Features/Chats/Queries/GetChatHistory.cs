using Hisubmit.Client.SharedModels.Enums.Chats;


namespace Hisubmit.Client.SharedModels.Features.Chats.Queries;

public class GetChatHistoryQuery
{
    public int RoomId { get; set; }
    public ChatRequestUserType Type { get; set; }
}

public class GetChatHistoryResponse
{
    public int Id { get; set; }
    public string Text { get; set; }
    public string UserId { get; set; }
    public ChatMessageType Type { get; set; }
    public int ChatRoomId { get; set; }
    public DateTime CreatedOn { get; set; }
}