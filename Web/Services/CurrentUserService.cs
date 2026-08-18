using System.Security.Claims;
using HiSubmit.Application.Interfaces.Services;
using Hisubmit.Client.SharedModels.Contracts.Permission;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string UserId =>
        User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

    public int? FestivalId
    {
        get
        {
            var claimValue = User?.FindFirst(ApplicationClaimTypes.FestivalId)?.Value;
            return int.TryParse(claimValue, out var id) ? id : null;
        }
    }

    public string? UserName =>
        User?.Identity?.Name;

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) =>
        User?.IsInRole(role) ?? false;

    public string? UserIP => _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
}


public class BaseUrlService(IConfiguration configuration):IBaseUrlService
{
    public string GetBaseUrl()
    {
        return configuration["SiteURLConfiguration:BaseUrl"] ?? string.Empty;
    }
}
