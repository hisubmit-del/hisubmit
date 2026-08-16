namespace HiSubmit.Application.Interfaces.Chat;

public interface ISeeMessagesRequest
{
    public  string SenderUserId { get; set; }
    public  string ReceiverUserId { get; set; }
}