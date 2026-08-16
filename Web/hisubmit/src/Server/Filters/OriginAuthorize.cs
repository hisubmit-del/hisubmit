using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace HiSubmit.Server.Filters;

public class OriginAuthorize :Attribute, IAuthorizationFilter
{
    private ISiteUrlService _siteUrlService;

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // _siteUrlService ??= context.HttpContext.RequestServices.GetService<ISiteUrlService>();
        //
        // var origin = context.HttpContext.Filter.Headers["Origin"].FirstOrDefault();
        // var baseUrl = _siteUrlService.GetBaseUrl().Remove(_siteUrlService.GetBaseUrl().Length - 1);
        //
        // if (!string.IsNullOrWhiteSpace(origin) && origin != baseUrl )
        //     context.Result = new ForbidResult();
    }
}