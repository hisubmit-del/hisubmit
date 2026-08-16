using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Models.Chat;
using System.Collections.Generic;
using HiSubmit.Application.Enums;

namespace HiSubmit.Application.Responses.Identity;

public class ChatUserResponse
{
    public string Id { get; set; }
    public string UserName { get; set; }
    //public  int? ProductFestivalId { get; set; }
    public  int? ToFestivalId { get; set; }
    public  int? FromFestivalId { get; set; }
    public string ProfilePictureDataUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
   // public  bool IsAdminUser { get; set; }
   public  bool AdminSender { get; set; }
   public bool AdminReceiver { get; set; }
    public string EmailAddress { get; set; }
    public bool IsOnline { get; set; }
    public  ChatUserType Type { get; set; }
        
    public virtual ICollection<ChatHistory<IChatUser>> ChatHistoryFromUsers { get; set; }
    public virtual ICollection<ChatHistory<IChatUser>> ChatHistoryToUsers { get; set; }
}