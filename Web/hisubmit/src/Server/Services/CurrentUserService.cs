using HiSubmit.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace HiSubmit.Server.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        if (int.TryParse(httpContextAccessor.HttpContext?.User?.FindFirstValue("ProductFestivalId"), out var festivalId))
        {
            FestivalId = festivalId;
        }

        UserIP = _httpContextAccessor.HttpContext?.Connection?.LocalIpAddress.ToString();
    }

    public string UserId { get; }
    public List<KeyValuePair<string, string>> Claims { get; set; }
    public int? FestivalId { get; }
    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext!.User.IsInRole(role);
    }

    public string UserIP { get; set; }
    public bool IsAuthenticated { get; }
}
