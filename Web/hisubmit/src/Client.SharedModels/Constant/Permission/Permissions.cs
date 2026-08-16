using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Hisubmit.Client.SharedModels.CustomeAttribute;

namespace Hisubmit.Client.SharedModels.Contracts.Permission;

public static class Permissions
{
    #region Admin Permission

    #region Admin Chat

    [CostumePermission(PermissionType.Admin)]
    [Display(Name = "Chat")]
    public static class AdminChat
    {
        [Display(Name = "View Chat", Description = "Allows viewing the chat messages")]
        public const string View = "Permissions.Chat.View";

        [Display(Name = "Send Chat Message", Description = "Allows sending messages in chat")]
        public const string SendMessage = "Permissions.Chat.SendMessage";
    }

    #endregion

    #region Art Category
    [Display(Name = "Art Category")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class ArtCategory
    {
        [Display(Name = "View Art Category", Description = "View Art Category list Page")]
        public const string View = "Permissions.ArtCategory.View";

        [Display(Name = "Create Art Category", Description = "Create new Art Categories")]
        public const string Create = "Permissions.ArtCategory.Create";

        [Display(Name = "Edit Art Category", Description = "Edit existing Art Categories")]
        public const string Edit = "Permissions.ArtCategory.Edit";

        [Display(Name = "Delete Art Category", Description = "Delete Art Categories")]
        public const string Delete = "Permissions.ArtCategory.Delete";

        [Display(Name = "Export Art Category", Description = "Export Art Category data")]
        public const string Export = "Permissions.ArtCategory.Export";

        [Display(Name = "Search Art Category", Description = "Search within Art Categories")]
        public const string Search = "Permissions.ArtCategory.Search";
    }

    #endregion

    #region Users
    [Display(Name = "Users")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class Users
    {
        [Display(Name = "View Users", Description = "View the list of users")]
        public const string View = "Permissions.Users.View";

        [Display(Name = "Create Users", Description = "Create new users")]
        public const string Create = "Permissions.Users.Create";

        [Display(Name = "Edit Users", Description = "Edit user details")]
        public const string Edit = "Permissions.Users.Edit";

        [Display(Name = "Delete Users", Description = "Delete users")]
        public const string Delete = "Permissions.Users.Delete";

        [Display(Name = "Export Users", Description = "Export users data")]
        public const string Export = "Permissions.Users.Export";

        [Display(Name = "Search Users", Description = "Search users")]
        public const string Search = "Permissions.Users.Search";
    }

    #endregion

    #region Roles
    [Display(Name = "Roles")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class Roles
    {
        [Display(Name = "View Roles", Description = "View roles list")]
        public const string View = "Permissions.Roles.View";

        [Display(Name = "Create Roles", Description = "Create new roles")]
        public const string Create = "Permissions.Roles.Create";

        [Display(Name = "Edit Roles", Description = "Edit existing roles")]
        public const string Edit = "Permissions.Roles.Edit";

        [Display(Name = "Delete Roles", Description = "Delete roles")]
        public const string Delete = "Permissions.Roles.Delete";

        [Display(Name = "Search Roles", Description = "Search roles")]
        public const string Search = "Permissions.Roles.Search";
    }

    #endregion


    #region Role Claims
    [Display(Name = "Role Claims")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class RoleClaims
    {
        [Display(Name = "View Role Claims", Description = "View the list of role claims")]
        public const string View = "Permissions.RoleClaims.View";

        [Display(Name = "Create Role Claims", Description = "Create new role claims")]
        public const string Create = "Permissions.RoleClaims.Create";

        [Display(Name = "Edit Role Claims", Description = "Edit existing role claims")]
        public const string Edit = "Permissions.RoleClaims.Edit";

        [Display(Name = "Delete Role Claims", Description = "Delete role claims")]
        public const string Delete = "Permissions.RoleClaims.Delete";

        [Display(Name = "Search Role Claims", Description = "Search within role claims")]
        public const string Search = "Permissions.RoleClaims.Search";
    }

    #endregion

    #region Communication
    [Display(Name = "Communication")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class Communication
    {
        [Display(Name = "Chat Communication", Description = "Access chat communication features")]
        public const string Chat = "Permissions.Communication.Chat";
    }

    #endregion

    #region Hangfire
    [Display(Name = "Hangfire")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    
    public static class Hangfire
    {
        [Display(Name = "View Hangfire Dashboard", Description = "View background jobs and Hangfire dashboard")]
        public const string View = "Permissions.Hangfire.View";
    }

    #endregion

    #region AuditTrails
    [Display(Name = "Audit Trails")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class AuditTrails
    {
        [Display(Name = "View Audit Trails", Description = "View system audit logs")]
        public const string View = "Permissions.AuditTrails.View";

        [Display(Name = "Export Audit Trails", Description = "Export audit logs")]
        public const string Export = "Permissions.AuditTrails.Export";

        [Display(Name = "Search Audit Trails", Description = "Search through audit logs")]
        public const string Search = "Permissions.AuditTrails.Search";
    }

    #endregion




    #region FocusCategory
    [Display(Name = "Focus Category")]
    [CostumePermission(PermissionType = PermissionType.Admin)]
    public static class FocusCategory
    {
        [Display(Name = "View Festival Focus", Description = "View Festival Focus categories")]
        public const string View = "Permissions.FestiivalFocus.View";

        [Display(Name = "Edit Festival Focus", Description = "Edit Festival Focus categories")]
        public const string Edit = "Permissions.FestiivalFocus.Edit";
    }

    #endregion

    #region Admin Festival

    [CostumePermission(PermissionType.Admin)]
    [Display(Name = "Festivals")]
    public static class AdminFestival
    {
        [Display(Name = "View Admin Festival", Description = "View admin festival details")]
        public const string View = "Permissions.AdminFestival.View";

        [Display(Name = "Activate Admin Festival", Description = "Activate a festival")]
        public const string Activate = "Permissions.AdminFestival.Activate";

        [Display(Name = "Change Fee Type", Description = "Change the fee type of a festival")]
        public const string ChangeFeeType = "Permissions.AdminFestival.ChangeFeeType";

        [Display(Name = "View Payment Item", Description = "View payment items for festival")]
        public const string ViewPaymentItem = "Permissions.AdminFestival.ViewPaymentItem";

        [Display(Name = "Update Payment Item", Description = "Update payment items for festival")]
        public const string UpdatePaymentItem = "Permissions.AdminFestival.UpdatePaymentItem";
    }

    #endregion

    #region ViolationReport

    [CostumePermission(PermissionType.Admin)]
    [DisplayName("ViolationReport")]
    public static class ViolationReport
    {
        [Display(Name = "View Violation Reports", Description = "View reports about violations")]
        public const string View = "Permissions.ViolationReport.View";
    }

    #endregion

    #region Advertise
    [DisplayName("Advertise")]
    [CostumePermission(PermissionType.Admin)]
    public static class Advertise
    {
        [Display(Name = "View Advertise Requests", Description = "View advertising requests")]
        public const string RequestView = "Permissions.Advertise.Request.View";

        [Display(Name = "View Advertise Banners", Description = "View advertising banners")]
        public const string BannerView = "Permissions.Advertise.Banner.View";

        [Display(Name = "Update Advertise Banners", Description = "Update advertising banners")]
        public const string BannerUpdate = "Permission.Advertise.Banner.Update";
    }

    #endregion

    #region Commission

    [CostumePermission(PermissionType.Admin)]
    [Display(Name="Site Commission")]
    public static class Commission
    {
        [Display(Name = "View Commission", Description = "View commission settings")]
        public const string View = "Permissions.Commission.View";

        [Display(Name = "Update Commission", Description = "Update commission settings")]
        public const string Update = "Permissions.Commission.Update";
    }

    #endregion


    #region Contents
    [Display(Name = "Static Pages/Menu items")]
    [CostumePermission(PermissionType.Admin)]
    
    public static class Contents
    {
        [Display(Name = "View Contents", Description = "View content pages")]
        public const string View = "Permissions.Content.View";

        [Display(Name = "Update New Content", Description = "Update new content entries")]
        public const string UpdateNew = "Permissions.Content.New.Update";

        [Display(Name = "Update Menu Item", Description = "Update content menu items")]
        public const string UpdateMenuItem = "Permissions.Content.MenuItem.Update";

        [Display(Name = "Update Static Page", Description = "Update static content pages")]
        public const string UpdateStaticPage = "Permissions.Content.StaticPage.Update";

      
        [Display(Name = "Update F&Q Page", Description = "Update F&Q  pages")]
        public const string UpdateFAQ = "Permissions.Content.UpdateFAQ.Update";
    }

    #endregion



    #endregion

    #region Festival Permission

    #region FestivalSeo

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Seo Setting")]
    public static class FestivalSeo
    {
        [Display(Name = "View Festival SEO", Description = "View SEO settings for festivals")]
        public const string View = "Permissions.FestivalSeo.View";

        [Display(Name = "Edit Festival SEO", Description = "Edit SEO settings for festivals")]
        public const string Edit = "Permissions.FestivalSeo.Edit";
    }

    #endregion

    #region Project
    [Display(Name = "Project")]
    [CostumePermission(PermissionType = PermissionType.Artist)]
    public static class Project
    {
        [Display(Name = "View Project", Description = "View projects")]
        public const string View = "Permissions.Project.View";

        [Display(Name = "Edit Project", Description = "Edit projects")]
        public const string Edit = "Permissions.Project.Edit";
    }

    #endregion

    #region Festival


    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Festival Info")]
    public static class Festival
    {
        [Display(Name = "View Festival", Description = "View festival details")]
        public const string View = "Permissions.Festiival.View";

        [Display(Name = "Edit Festival", Description = "Edit festival details")]
        public const string Edit = "Permissions.Festiival.Edit";
    }

    #endregion

    #region Submits

    [Display(Name = "Submissions")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class Submits
    {
        [Display(Name = "View Submissions", Description = "View submissions for festival")]
        public const string View = "Permissions.Festival.Submit.View";

        [Display(Name = "Add to Referee", Description = "Assign submissions to referees")]
        public const string AddToReferee = "Permissions.Festival.Submit.AddToReferee";

        [Display(Name = "View Final Result", Description = "View final results of submissions")]
        public const string FinalResult = "Permissions.Festival.Submit.FinalResult";
    }

    #endregion





    #region Judging Form
    [Display(Name = "Judging Form")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class JudgingForm
    {
        [Display(Name = "View Judging Form", Description = "View judging forms")]
        public const string View = "Permissions.JudgingForm.View";

        [Display(Name = "Edit Judging Form", Description = "Edit judging forms")]
        public const string Edit = "Permmissions.JudgingForm.Edit";  
    }

    #endregion

    #region SubmissionForm
    [Display(Name = "Submission Form")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class SubmissionForm
    {
        [Display(Name = "View Submission Form", Description = "View submission forms")]
        public const string View = "Permissions.SubmissionForm.View";  
        [Display(Name = "Edit Submission Form", Description = "Edit submission forms")]
        public const string Edit = "Permissions.SubmissionForm.Edit";  
    }

    #endregion

    #region Festival Chat
    [Display( Name="Festival Chats")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class FestivalChat
    {
        [Display(Name = "View Festival Chat", Description = "View chat in festival")]
        public const string View = "Permissions.FestivalChat.View";

        [Display(Name = "Send Festival Chat Message", Description = "Send chat messages in festival")]
        public const string Send = "Permissions.FestivalChat.Send";
    }

    #endregion

    #region Festival News

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Festival News")]
    public static class FestivalNews
    {
        [Display(Name = "View Festival News", Description = "View news related to festival")]
        public const string View = "Permissions.FestivalNews.View";

        [Display(Name = "Edit Festival News", Description = "Edit news related to festival")]
        public const string Edit = "Permissions.FestivalNews.Edit";
    }

    #endregion



    #region Judging

    [CostumePermission(PermissionType.Festival)]
    [Display(Name = "Judging")]
    public static class Judging
    {
        [Display(Name = "View Judging", Description = "View judging information")]
        public const string View = "Permissions.Judging.View";

        [Display(Name = "Edit Judging", Description = "Edit judging information")]
        public const string Edit = "Permissions.Judging.Edit";
    }

    #endregion

    #region Reviews

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Festival Reviews")]
    public static class Reviews
    {
        [Display(Name = "View Reviews", Description = "View reviews")]
        public const string View = "Permissions.Reviews.View";
    }

    #endregion

    #region FestivalProduct
    [Display(Name = "Festival Products")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class FestivalProducts
    {
        [Display(Name = "View Festival Products", Description = "View festival products")]
        public const string View = "Permissions.FestivalProducts.View";

        [Display(Name = "Edit Festival Products", Description = "Edit festival products")]
        public const string Edit = "Permissions.FestivalProducts.Edit";
    }

    #endregion

    #region Subuser

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Staff Management")]
    public static class SubUser
    {
        [Display(Name = "View Staff", Description = "View festival Staff")]
        public const string View = "Permission.SubUser.View";

        [Display(Name = "Create New Staff", Description = "Create a new Staff for festival")]
        public const string CreateNewUser = "Permission.SubUser.CreateNewUser";

        [Display(Name = "Add Existing User", Description = "Add an existing user as Staff")]
        public const string AddExistUser = "Permission.SubUser.AddExistUser";

        [Display(Name = "Add Staff to Project", Description = "Assign Staff to project")]
        public const string AddToProject = "Permission.SubUser.AddToProject";

        [Display(Name = "Manage Staff Roles", Description = "Manage roles of Staff")]
        public const string ManageRoles = "Permission.SubUser.ManageRoles";

        [Display(Name = "Remove Staff from Festival", Description = "Remove Staff from festival")]
        public const string RemoveFestival = "Permission.SubUser.RemoveFromFestival";
    }

    #endregion




    #region Subuser Roles

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Staff Roles")]
    public static class SubUserRole
    {
        [Display(Name = "View Roles", Description = "View roles of festival Staff")]
        public const string View = "Permission.SubUserRole.View";

        [Display(Name = "Edit  Roles", Description = "Edit roles of festival Staff")]
        public const string Edit = "Permission.SubUserRole.Edit";
    }

    #endregion

    #region Venue

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Venue")]
    public static class Venue
    {
        [Display(Name = "View Venue", Description = "View festival venues")]
        public const string View = "Permission.Venue.View";

        [Display(Name = "Edit Venue", Description = "Edit festival venues")]
        public const string Edit = "Permission.Venue.Edit";
    }

    #endregion

    #region Ticket

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Ticket")]
    public static class Ticket
    {
        [Display(Name = "View Ticket", Description = "View festival tickets")]
        public const string View = "Permission.Ticket.View";

        [Display(Name = "Edit Ticket", Description = "Edit festival tickets")]
        public const string Edit = "Permission.Ticket.Edit";
    }

    #endregion

    #region Ticket Sold

    [CostumePermission(PermissionType = PermissionType.Festival)]
    [Display(Name = "Sold Tickets")]
    public static class SoldTickets
    {
        [Display(Name = "View Sold Tickets", Description = "View tickets sold for festival")]
        public const string View = "Permission.TicketsSold.View";

        [Display(Name = "Edit Sold Tickets", Description = "Edit tickets sold information")]
        public const string Edit = "Permission.TicketsSold.Edit";
    }

    #endregion

    #region Festival Payment
    [Display(Name = "Accounting Department")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class FestivalPayment
    {
        [Display(Name = "View Cart Items", Description = "View items in festival cart")]
        public const string CartItem = "Permission.FestivalPayment.CartItem";

        [Display(Name = "View Payment Information", Description = "View festival payment details")]
        public const string PaymentInformation = "Permission.Festival.PaymentInformation";
    }

    #endregion

    #region Discount Code
    [Display(Name = "Discount Code")]
    [CostumePermission(PermissionType = PermissionType.Festival)]
    public static class DiscountCode
    {
        [Display(Name = "View Discount Codes", Description = "View discount codes for festival")]
        public const string View = "Permission.DiscountCode.View";

        [Display(Name = "Edit Discount Codes", Description = "Edit discount codes for festival")]
        public const string Edit = "Permission.DiscountCode.Edit";
    }

    #endregion



    #endregion


    /// <summary>
    /// Returns a list of Permissions.
    /// </summary>
    /// <returns></returns>
    /// 
    public static List<string> GetRegisteredAdminPermissions()
    {
        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes()
                     .Where(p => p.GetCustomAttribute<CostumePermissionAttribute>()?.PermissionType ==
                                 PermissionType.Admin)
                     .SelectMany(c =>
                         c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString());
        }

        return permissions;
    }

    public static List<string> GetRegisteredFestivalPermissions()
    {

        var permissions = new List<string>();
        foreach (var prop in typeof(Permissions).GetNestedTypes()
                     .Where(p => p.GetCustomAttribute<CostumePermissionAttribute>()?.PermissionType ==
                                 PermissionType.Festival)
                     .SelectMany(c =>
                         c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        {
            var propertyValue = prop.GetValue(null);
            if (propertyValue is not null)
                permissions.Add(propertyValue.ToString());
        }

        return permissions;
    }
}