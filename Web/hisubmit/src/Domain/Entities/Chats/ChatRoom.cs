using System;
using System.ComponentModel.DataAnnotations.Schema;
using HiSubmit.Domain.Contracts;
using HiSubmit.Domain.Entities.Festivals;
using HiSubmit.Domain.Enums.Chats;

namespace HiSubmit.Domain.Entities.Chats;

public class ChatRoom:AuditableEntity<int>
{
    public  string ChatUser1 { get; set; }
    public string ChatUser2 { get; set; }
    public  bool ChatWithAdmin { get; set; }
    [ForeignKey(nameof(Festival))]
    public  int? FestivalId { get; set; }
    public  ChatRoomType Type { get; set; }
    public  Festival Festival { get; set; }
    public  DateTime LastModifiedTime { get; set; }
}

