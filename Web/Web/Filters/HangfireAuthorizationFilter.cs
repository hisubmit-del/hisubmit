using Hangfire.Dashboard;
using System.Security.Claims;
using HiSubmit.Client.SharedModels.Constants.Role;

namespace Web.Filters;

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
