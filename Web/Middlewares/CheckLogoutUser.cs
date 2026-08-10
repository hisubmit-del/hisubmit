using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares;

public class CheckLogoutUser(RequestDelegate next) : Controller
{
    public async Task Invoke(HttpContext context)
    {
        await next(context);

        if (context.User.Identity?.IsAuthenticated == false &&
            context.Request.Cookies.ContainsKey(ApplicationClaimTypes.SelectedFestival))
            context.Response.Cookies.Delete(ApplicationClaimTypes.SelectedFestival);

        if (context.User.Identity?.IsAuthenticated == false &&
            context.Request.Cookies.ContainsKey(ApplicationClaimTypes.AdminLoginFestival))
            context.Response.Cookies.Delete(ApplicationClaimTypes.AdminLoginFestival);

    }
}