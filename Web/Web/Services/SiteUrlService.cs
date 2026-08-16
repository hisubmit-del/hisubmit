using HiSubmit.Application.Configurations;
using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Web.Services;

public class SiteUrlService(IOptions<SiteURLConfiguration> siteUrlOption) : ISiteUrlService
{
    public string GetBaseUrl()
    {
        return siteUrlOption.Value.BaseUrl;
    }
}

// Extension Method در فایل جداگانه
public static class PrerenderingHelper
{
    public static bool IsPrerendering(this IHttpContextAccessor contextAccessor)
    {
        return contextAccessor.HttpContext?.Response.HasStarted == false;
    }
}
