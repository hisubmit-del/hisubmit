using System;

namespace HiSubmit.Client.SharedModels.Constants.Application;

public static class ApplicationConstants
{
    public static class SignalR
    {
        public const string HubUrl = "/signalRHub";
        public const string SendUpdateDashboard = "UpdateDashboardAsync";
        public const string ReceiveUpdateDashboard = "UpdateDashboard";
        public const string SendRegenerateTokens = "RegenerateTokensAsync";
        public const string ReceiveRegenerateTokens = "RegenerateTokens";
        public const string ReceiveChatNotification = "ReceiveChatNotification";
        public const string SendChatNotification = "ChatNotificationAsync";
        public const string ReceiveMessage = "ReceiveMessage";
        public const string SendMessage = "SendMessageAsync";

        public const string OnConnect = "OnConnectAsync";
        public const string ConnectUser = "ConnectUser";
        public const string OnDisconnect = "OnDisconnectAsync";
        public const string DisconnectUser = "DisconnectUser";
        public const string OnChangeRolePermissions = "OnChangeRolePermissions";
        public const string LogoutUsersByRole = "LogoutUsersByRole";

        public const string ReceiveMessageUser = "ReceiveMessageUser";
        public const string ReceiveMessageAdmin = "ReceiveMessageAdmin";
        public const string ReceiveMessageFestival = "ReceiveMessageFestival";
        
        public const string ReceiveMessageUserNotification = "ReceiveMessageUserNotification";
        public const string ReceiveMessageAdminNotification = "ReceiveMessageUserNotification";
        public const string ReceiveMessageFestivalNotification = "ReceiveMessageUserNotification";
        
        public const string SendMessageToUser = "SendMessageToUser";
        public const string SendMessageToAdmin = "SendMessageToAdmin";
        public const string SendMessageToFestival = "SendMessageToFestival";
    }
    public static class Cache
    {
        public const string GetAllBrandsCacheKey = "all-brands";
        public const string GetAllDocumentTypesCacheKey = "all-document-types";
        #region FestivalId
        public const string GetAllFestivalCacheKey = "all-festival";
        public const string GetAllEventOrginizerKey = "all-event-orginizer";
        public const string GetAllDeadLineCacheKey = "all-deadLine";
        public const string GetAllVenueCacheKey = "all-venue";
        public const string GetAllFestivalFocusCacheKey = "festival-focus";
        #endregion

        #region Locations
        public const string GetAllAddressCachKey = "all-address";
        public const string GetAllCountryCachKey = "all-country";
        public const string GetAllEventCategoryCacheKefy = "all-eventCategory";
        public const string GetAllsubmissionQuestion = "all-submission-question";
        #endregion
        public static string GetAllEntityExtendedAttributesCacheKey(string entityFullName)
        {
            return $"all-{entityFullName}-extended-attributes";
        }

        public static string GetAllEntityExtendedAttributesByEntityIdCacheKey<TEntityId>(string entityFullName, TEntityId entityId)
        {
            return $"all-{entityFullName}-extended-attributes-{entityId}";
        }

        public const string GetAllFooterItem = "all-footer-item";
    }

    public static class MimeTypes
    {
        public const string OpenXml = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string Pdf = "application/pdf";
    }


    public static class  Claims
    {
        public const string FestivalId = "FestivalId";
    }
}