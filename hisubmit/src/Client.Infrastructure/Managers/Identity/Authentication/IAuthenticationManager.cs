using System.Collections.Generic;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Security.Claims;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Identity;
using Hisubmit.Client.SharedModels.Responses.Identity;

namespace HiSubmit.Client.Infrastructure.Managers.Identity.Authentication
{
    public interface IAuthenticationManager : ITransientManager
    {
        Task<IResult<TokenResponse>> Login(TokenRequest model);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
        Task<IResult> VerifyEmail(VerificationCodeRequest codeRequest);
        Task<IResult> ResendVerifyEmail(ResendVerificationCodeRequest codeRequest);

        int? GetMainFestivalId();
        IEnumerable<int> GetOtherFestivalId();
        Task<int?> GetSelectedFestivalId();
        Task<bool> IsPersonalAccountSelected();
        Task<int?> GetAdminLoginToFestivalId();
    }
}
