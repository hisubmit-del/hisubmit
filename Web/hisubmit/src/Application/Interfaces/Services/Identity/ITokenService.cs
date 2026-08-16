using HiSubmit.Application.Interfaces.Common;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}