
using Hisubmit.Client.SharedModels.Enums.Chats;

namespace Hisubmit.Client.SharedModels.Features.Chats.Queries;

public class GetAllRoomQuery 
{
    public int? FestivalId { get; set; }
    public string UserId { get; set; }
    public ChatRequestUserType RequestUserType { get; set; }
}


public class GetAllRoomResponse
{
    public int RoomId { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    
    public  ChatRoomType Type { get; set; }
    public string Title { get; set; }
    public int NotSeenMessageCount { get; set; }
}

