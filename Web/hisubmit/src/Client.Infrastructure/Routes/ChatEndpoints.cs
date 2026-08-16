using System;

namespace HiSubmit.Client.Infrastructure.Routes
{
    public static class ChatEndpoint
    {
        public static string GetAvailableUsers = "api/chats/users";
        public static string SaveMessage = "api/chats";

        public static string GetChatHistory(string userId,int? festivalId,bool forSiteAdmin=false)
        {
            Console.WriteLine("Get ChatHistor33");
            Console.WriteLine(forSiteAdmin);

            return $"api/chats/chatHistory?contactId={userId}&festivalId={festivalId}&fsa={forSiteAdmin}";
        }


        public static string GetChatMessages = "api/chats/chatMessages";
        public static string AllRooms = "api/chats/rooms";
    }
}