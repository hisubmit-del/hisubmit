using HiSubmit.Application.Interfaces.Chat;
using System;

namespace HiSubmit.Application.Models.Chat;

public partial class ChatHistory<TUser> : IChatHistory<TUser> where TUser : IChatUser
{
    public long Id { get; set; }
    public string FromUserId { get; set; }
    public string? ToUserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
    public virtual TUser FromUser { get; set; }
    public virtual TUser ToUser { get; set; }
    public int? FromFestivalId { get; set; }
    public int? ToFestivalId { get; set; }
    public bool AdminSender { get; set; }
    public bool AdminReceiver { get; set; }
    // public int? ProductFestivalId { get; set; }
    // public bool ForSiteAdmins { get; set; }
    public bool Seen { get; set; }
}