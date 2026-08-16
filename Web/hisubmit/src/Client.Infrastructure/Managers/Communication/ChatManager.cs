using Hisubmit.Client.SharedModels.Interfaces.Chat;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using Hisubmit.Client.SharedModels.Models.Chat;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.Infrastructure.Routes;

namespace HiSubmit.Client.Infrastructure.Managers.Communication;

public class ChatManager : IChatManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public ChatManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/chats");
    }

    public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int? festivalId,bool forSiteAdmin)
    {
        var response = await _httpClient.GetAsync(Routes.ChatEndpoint.GetChatHistory(cId,festivalId,forSiteAdmin));
        var data = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
        return data;
    }

    public async Task<IResult<List<GetAllRoomResponse>>> GetAllRooms(GetAllRoomQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("rooms", query));
        return await response.ToResult<List<GetAllRoomResponse>>();
    }

    public async Task<IResult> AddMessage(AddChatMessageRequest message)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("AddMessage"),message);
        return await response.ToResult();
    }

    public async Task<IResult<List<GetChatHistoryResponse>>> GetAllChatMessage(GetChatHistoryQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("ChatMessages", query));
        return await response.ToResult<List<GetChatHistoryResponse>>();
    }

    public async Task<IResult<List<GetAllContactResponse>>> GetAllContact(GetAllContactQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("contacts", query));
        return await response.ToResult<List<GetAllContactResponse>>();
    }

    public async Task<IResult<int>> GetRoomId(TryGetRoomIdCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("GetRoomId"), command);
        return await response.ToResult<int>();
    }

    public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
    {
        var response = await _httpClient.GetAsync(Routes.ChatEndpoint.GetAvailableUsers);
        var data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory)
    {
        var response = await _httpClient.PostAsJsonAsync(Routes.ChatEndpoint.SaveMessage, chatHistory);
        var data = await response.ToResult();
        return data;
    }
}