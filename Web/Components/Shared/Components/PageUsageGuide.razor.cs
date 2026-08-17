using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;

namespace Web.Components.Shared.Components;

public partial class PageUsageGuide : IDisposable
{
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    private GuideContent? _guide;

    protected override void OnInitialized()
    {
        NavigationManager.LocationChanged += OnLocationChanged;
        _guide = ResolveGuide(NavigationManager.ToBaseRelativePath(NavigationManager.Uri));
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        _guide = ResolveGuide(NavigationManager.ToBaseRelativePath(args.Location));
        InvokeAsync(StateHasChanged);
    }

    private static GuideContent? ResolveGuide(string relativePath)
    {
        var path = "/" + relativePath.Split('?', '#')[0].Trim('/');
        var lower = path.ToLowerInvariant();

        if (lower is "/" or "/home")
            return null;

        if (lower.StartsWith("/user/projects"))
            return New("My projects", "Create, search and manage the works you want to submit. Open a project to update its information or media.", Icons.Material.Outlined.Folder);
        if (lower == "/user/project")
            return New("Create a project", "Enter the work details and upload its media. Save the project first, then continue with festival submission when it is complete.", Icons.Material.Outlined.Add);
        if (lower.StartsWith("/user/project/"))
            return New("Your project", "Complete the project details, upload the correct media, then save each section before submitting it to a festival.", Icons.Material.Outlined.Movie);
        if (lower == "/user/dashboard")
            return New("Artist dashboard", "Review your projects, submissions, notifications and account activity. Select a project card to edit it.", Icons.Material.Outlined.Dashboard);
        if (lower.StartsWith("/user/submits"))
            return New("My submissions", "Track every festival submission, its payment status and judging progress. Open a row for the available actions.", Icons.Material.Outlined.Approval);
        if (lower.StartsWith("/user/shoppingcart"))
            return New("Shopping cart", "Review submission fees, tickets and products. Remove unwanted items or continue to checkout when everything is correct.", Icons.Material.Outlined.ShoppingCart);
        if (lower.StartsWith("/user/tickets") || lower.StartsWith("/user/purchasedproducts"))
            return New("Your purchases", "Review purchased tickets, products and entry documents. Use the available actions to open or download each item.", Icons.Material.Outlined.Receipt);
        if (lower.StartsWith("/account"))
            return New("Your profile", "Update your personal details, profile picture and account preferences. Save the form after each change.", Icons.Material.Outlined.AccountCircle);

        if (lower == "/festival/dashboard")
            return New("Festival dashboard", "Monitor submissions, deadlines, judging, sales and audience activity from one operational overview.", Icons.Material.Outlined.Dashboard);
        if (lower.StartsWith("/festival/edit"))
            return New("Festival editor", "Complete the festival information step by step. Save a section before moving to the next tab.", Icons.Material.Outlined.Edit);
        if (lower.StartsWith("/festival/submits"))
            return New("Festival submissions", "Filter submitted works, open their project pages and manage the review or judging workflow.", Icons.Material.Outlined.Approval);
        if (lower.StartsWith("/festival/judging") || lower.StartsWith("/festival/projectjudging"))
            return New("Judging workspace", "Define judging fields or review assigned works. Only authorized festival roles can see and change these records.", Icons.Material.Outlined.Score);
        if (lower.StartsWith("/festival/products"))
            return New("Festival store", "Create and manage products for your festival. Add clear descriptions, pricing and images before publishing.", Icons.Material.Outlined.Storefront);
        if (lower.StartsWith("/festival/tickets") || lower.StartsWith("/festival/venues") || lower.StartsWith("/festival/soldtickets"))
            return New("Tickets and venues", "Set up venues and ticket types, then review availability and sold-ticket information.", Icons.Material.Outlined.LocalActivity);
        if (lower.StartsWith("/festival/news"))
            return New("Festival news", "Write, schedule and publish updates for your festival audience. Check the preview before publishing.", Icons.Material.Outlined.Newspaper);
        if (lower.StartsWith("/festival/chat") || lower.StartsWith("/festival/reviews"))
            return New("Festival communication", "Respond to messages and audience feedback while keeping festival communication organized.", Icons.Material.Outlined.Forum);
        if (lower.StartsWith("/festival/subuser") || lower.StartsWith("/festival/roles"))
            return New("Festival team access", "Invite team members and assign only the permissions they need for this festival or judging season.", Icons.Material.Outlined.SupervisorAccount);
        if (lower.StartsWith("/festival/payment") || lower.StartsWith("/festival/discount"))
            return New("Festival finances", "Review payment settings, discounts and income-related records for the selected festival.", Icons.Material.Outlined.Payments);
        if (lower.StartsWith("/festival/"))
            return New("Festival workspace", "Use the selected festival workspace to manage its content, submissions, judging and sales.", Icons.Material.Outlined.Event);

        if (lower.StartsWith("/admin"))
            return New("Administration", "Manage site-wide content, users, festivals, permissions and financial records. Changes here can affect the whole platform.", Icons.Material.Outlined.AdminPanelSettings);
        if (lower == "/advertise")
            return New("Promote your festival", "Submit an advertising request with your campaign details and preferred placement. The administration team will review it.", Icons.Material.Outlined.Campaign);
        if (lower == "/faq")
            return New("Help and FAQ", "Search the common questions or open a question to read its answer. Use the links in each answer to continue to the relevant page.", Icons.Material.Outlined.Help);
        if (lower.StartsWith("/store"))
            return New("Store", "Browse products from festivals, open an item for details and add it to your cart when you are ready.", Icons.Material.Outlined.Storefront, false);
        if (lower.StartsWith("/tickets"))
            return New("Tickets", "Browse available festival tickets, compare event details and add the tickets you want to purchase.", Icons.Material.Outlined.LocalActivity, false);
        if (lower.StartsWith("/news"))
            return New("News", "Browse platform and festival news. Open an article to read the full story and available festival links.", Icons.Material.Outlined.Newspaper, false);
        if (lower.StartsWith("/project/"))
            return New("Work detail", "Review the work, its media and credits. If you own it or have permission, use Edit Project Information to update it.", Icons.Material.Outlined.Visibility, false);
        if (lower.StartsWith("/festivalpage/") || IsPublicFestivalDetailPath(lower))
            return New("Festival detail", "Review deadlines, rules, fees, awards and tickets for this festival. Choose a category or action below to continue.", Icons.Material.Outlined.Event, false);
        if (lower.StartsWith("/new/"))
            return New("News article", "Read the full festival or platform update and use the available links to open related information.", Icons.Material.Outlined.Newspaper, false);
        if (lower.StartsWith("/product/"))
            return New("Product detail", "Review the product information, price and festival details, then add it to your cart if you want to purchase it.", Icons.Material.Outlined.ShoppingBag, false);
        if (lower.StartsWith("/chat") || lower.StartsWith("/admin/chat") || lower.StartsWith("/festival/chat"))
            return New("Messages", "Choose a conversation to read messages and reply. Keep sensitive festival or judging information inside the authorized conversation.", Icons.Material.Outlined.Chat);
        if (lower.StartsWith("/judges") || lower.StartsWith("/user/judges"))
            return New("Judging dashboard", "Open an assigned festival season, review the permitted works and submit each score or decision before leaving.", Icons.Material.Outlined.Score);

        if (lower.StartsWith("/user/"))
            return New("Your workspace", "Use this page to review your account data and complete the available actions. Save each change before leaving the page.", Icons.Material.Outlined.Dashboard);
        if (lower.StartsWith("/festival/"))
            return New("Festival workspace", "Use this page to manage the selected festival. Complete the form or use the page actions, then save before moving on.", Icons.Material.Outlined.Event);
        if (lower.StartsWith("/authentication/") || lower.StartsWith("/account/") || lower is "/login" or "/register")
            return New("Account access", "Complete the form and submit it to continue. Required fields are marked with a red asterisk.", Icons.Material.Outlined.AccountCircle);

        return null;
    }

    private static bool IsPublicFestivalDetailPath(string path)
    {
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return segments.Length == 2
            && segments[0].Equals("festival", StringComparison.OrdinalIgnoreCase);
    }

    private static GuideContent New(string title, string description, string icon, bool showRequiredLegend = true)
        => new(title, description, icon, showRequiredLegend);

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }

    private sealed record GuideContent(string Title, string Description, string Icon, bool ShowRequiredLegend);
}
