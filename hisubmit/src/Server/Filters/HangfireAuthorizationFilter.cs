using Hangfire.Dashboard;
using MailKit.Net.Imap;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using HiSubmit.Client.SharedModels.Constants.Role;

namespace HiSubmit.Server.Filters;

public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var currentUser = context.GetHttpContext().User;
        var roles = context.GetHttpContext()
            .User.Claims.Where(p=>p.Type==ClaimTypes.Role).ToList().Count;
        //  var roles = currentUser.Identity.IsAuthenticated;
        var isAdminRole = currentUser.IsInRole(RoleConstants.AdministratorRole);
        return true;
    }
}
