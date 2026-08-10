
namespace Hisubmit.Client.SharedModels.Features.Chats.Queries;

public class GetAllContactQuery 
{
    public ChatRequestUserType Type { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
}


public class GetAllContactResponse
{
    public string FullName { get; set; }
    public string ImageUrl { get; set; }
    public int? RoomId { get; set; }
    public ContactType ContactType { get; set; }
    public string UserId { get; set; }
    public int? FestivalId { get; set; }
    public  int? NotSeenCount { get; set; }


    public static GetAllContactResponse GetAdminContact()
    {
        return new GetAllContactResponse
        {
            ContactType = ContactType.Admin,
            ImageUrl = string.Empty,
            FullName = "Site Admin"
        };
    }
}

public enum ContactType
{
    Admin,
    Actors,
    Referee,
    Festival,
    OtherSubUser
}