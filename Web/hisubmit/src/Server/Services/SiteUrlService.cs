using HiSubmit.Application.Configurations;
using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HiSubmit.Server.Services;

public class SiteUrlService:ISiteUrlService
{
    private readonly IOptions<SiteURLConfiguration> _siteUrlOption;
    public SiteUrlService(IOptions<SiteURLConfiguration> siteUrlOption)
    {
        _siteUrlOption = siteUrlOption;
    }
    public string GetBaseUrl()
    {
        return _siteUrlOption.Value.BaseUrl;
    }
}

