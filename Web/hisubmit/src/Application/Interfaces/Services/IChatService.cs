using HiSubmit.Application.Interfaces.Chat;
using HiSubmit.Application.Models.Chat;
using HiSubmit.Application.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using HiSubmit.Application.Enums;

namespace HiSubmit.Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<IEnumerable<ChatUserResponse>>> GetAdminChatUsersAsync(string userId);

        Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> message);

        Task<Result<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string userId, string contactId,bool forSiteAdmin,int?festivalId=null);
        Task<Result<IEnumerable<ChatUserResponse>>> GetFestivalChatUserAsync(int festivalId);
        Task<Result<IEnumerable<ChatUserResponse>>> GetUserChatUsersAsync(string userId);
    }
}