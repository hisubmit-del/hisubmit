
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Requests.Identity;

namespace HiSubmit.Client.Infrastructure.Managers.Identity.Account
{
    public interface IAccountManager : ITransientManager
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}