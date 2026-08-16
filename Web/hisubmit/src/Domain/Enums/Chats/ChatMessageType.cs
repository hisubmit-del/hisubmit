namespace HiSubmit.Domain.Enums.Chats;

public enum ChatMessageType
{
    UserToUser,
    UserToAdmin,
    UserToFestival,
    FestivalToUser,
    FestivalToAdmin,
    FestivalToFestival,
    AdminToUser,
    AdminToFestival,
}