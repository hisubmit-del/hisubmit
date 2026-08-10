using Hisubmit.Client.SharedModels.Enums.Chats;

namespace Hisubmit.Client.SharedModels.Features.Chats.Commands;

public class TryGetRoomIdCommand 
{
    public string ChatUser1 { get; set; }
    public string ChatUser2 { get; set; }
    public bool ChatWithAdmin { get; set; }
    public int? FestivalId { get; set; }
    public ChatRoomType Type { get; set; }
}

