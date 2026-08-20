using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.AspNetCore.Mvc.Filters;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace Web.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class FestivalAuthentication : Attribute, IAuthorizationFilter
{
    public string Policy { get; set; }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            if (context.HttpContext.User.IsInRole(RoleConstants.AdministratorRole)) return;

            var claims = context.HttpContext.User.Claims;

            if (context.RouteData.Values.TryGetValue("festivalId", out var routeFestivalId) &&
                int.TryParse(routeFestivalId?.ToString(), out var festivalId))
            {
                var festivalClaims = claims.FirstOrDefault(p => p.Type == ApplicationClaimTypes.FestivalPermission);
                if (festivalClaims != null)
                {
                    var permissions = JsonSerializer.Deserialize<Dictionary<int, string[]>>(festivalClaims.Value);
                    if (permissions != null && permissions.ContainsKey(festivalId))
                    {
                        if (permissions[festivalId].Any(p => p == Policy))
                            return;
                    }
                }

                //var userFestivalClaims =
                //    claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission)
                //    .Select(p => p.Value).Select(p => p.Split('-'));

                //if (userFestivalClaims.Any(p => int.Parse(p[0]) == festivalId && p[1] == Policy))
                //    return;
            }
            context.Result = new ForbidResult();
            return;
        }

        context.Result = new UnauthorizedResult();
    }
}
