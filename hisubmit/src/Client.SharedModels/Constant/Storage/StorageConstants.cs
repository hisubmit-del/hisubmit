namespace Hisubmit.Client.SharedModels.Constants.Storage;

public static class StorageConstants
{
    public static class Local
    {
        public static readonly string Preference = "clientPreference";

        public static readonly string AuthToken = "authToken";
        public static readonly string RefreshToken = "refreshToken";
        public static readonly string UserImageURL = "userImageURL";
        public static readonly string FestivalId = "festivalId";
        public static readonly string SelectedFestivalId = "selected-festival-id";
        public static readonly string AdminSelectedFestivalId = "admin-selected-festival-id";
        public static readonly string ExpireToken = "expire-token";
        public static readonly string EmailRegistered = "email-registered";
        public static readonly string FestivalFilmFreewayUrl = "festival-filmfreeway-url";
    }

    public static class Server
    {
        public static readonly string Preference = "serverPreference";

        //TODO - add
    }
}
