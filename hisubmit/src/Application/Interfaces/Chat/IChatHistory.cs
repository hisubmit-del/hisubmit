using System;

#nullable enable

namespace HiSubmit.Application.Interfaces.Chat;

public interface IChatHistory<TUser> where TUser : IChatUser
{
    public long Id { get; set; }
    public string FromUserId { get; set; }
    public string? ToUserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedDate { get; set; }
    public TUser FromUser { get; set; }
    public TUser ToUser { get; set; } 
   // public  int? ProductFestivalId { get; set; }
    public  int? FromFestivalId { get; set; }
    public  int? ToFestivalId { get; set; }
  //  public  bool ForSiteAdmins { get; set; }
    public  bool AdminSender { get; set; }
    public  bool AdminReceiver { get; set; }
    public bool Seen { get; set; }
}
