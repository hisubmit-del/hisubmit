using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using HiSubmit.Client.SharedModels.Constants.Role;
using Microsoft.AspNetCore.Mvc.Filters;
using Hisubmit.Client.SharedModels.Contracts.Permission;

namespace HiSubmit.Server.Filters;

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
            var pathArray = context.HttpContext.Request.Path.Value?.Split('/');

            if (pathArray != null)
            {
                int? festivalId = int.Parse(pathArray[4]);
                var userFestivalId = context.HttpContext.User.Claims.FirstOrDefault(p => p.Type == "FestivalId");

                if (festivalId.ToString() == userFestivalId?.Value)
                    return;

                var userFestivalClaims = claims.Where(p => p.Type == ApplicationClaimTypes.FestivalPermission)
                    .Select(p => p.Value).Select(p => p.Split('-'));

                if (userFestivalClaims.Any(p => int.Parse(p[0]) == festivalId && p[1] == Policy))
                    return;
            }

            context.Result = new ForbidResult();
        }
        context.Result = new UnauthorizedResult();
    }
}