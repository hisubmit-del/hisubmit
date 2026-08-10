using Hisubmit.Client.SharedModels.Interfaces.Chat;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using Hisubmit.Client.SharedModels.Models.Chat;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Communication;

public interface IChatManager : ITransientManager
{
    Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

    Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int? festivalId,bool forSiteAdmin);
    Task<IResult<List<GetAllRoomResponse>>> GetAllRooms(GetAllRoomQuery query);
    Task<IResult> AddMessage(AddChatMessageRequest message);
    Task<IResult<List<GetChatHistoryResponse>>> GetAllChatMessage(GetChatHistoryQuery query);
    Task<IResult<List<GetAllContactResponse>>> GetAllContact(GetAllContactQuery query);
    Task<IResult<int>> GetRoomId(TryGetRoomIdCommand command);
}