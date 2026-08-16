using HiSubmit.Application.Interfaces.Common;

namespace HiSubmit.Application.Interfaces.Services
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
        int? FestivalId { get; }

        bool IsInRole(string role);
        string? UserIP { get;  }
        bool IsAuthenticated { get; }
    }

    public interface IBaseUrlService
    {
        string GetBaseUrl();
    }
}
