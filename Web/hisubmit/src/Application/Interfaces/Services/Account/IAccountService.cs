using HiSubmit.Application.Interfaces.Common;
using HiSubmit.Application.Requests.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;

namespace HiSubmit.Application.Interfaces.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}