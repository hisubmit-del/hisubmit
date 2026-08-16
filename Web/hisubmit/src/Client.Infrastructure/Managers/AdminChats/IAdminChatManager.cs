using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Chats.Commands;
using Hisubmit.Client.SharedModels.Features.Chats.Queries;
using Hisubmit.Client.SharedModels.Interfaces.Chat;
using Hisubmit.Client.SharedModels.Models.Chat;
using Hisubmit.Client.SharedModels.Responses.Identity;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.AdminChats;

public interface IAdminChatManager:ITransientManager
{
    Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync();

    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory);

    Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int? festivalId);
    
    Task<IResult<List<GetAllRoomResponse>>> GetAllRooms(GetAllRoomQuery query);
    Task<IResult> AddMessage(AddChatMessageRequest message);
    Task<IResult<List<GetChatHistoryResponse>>> GetAllChatMessage(GetChatHistoryQuery query);
    Task<IResult<List<GetAllContactResponse>>> GetAllContact(GetAllContactQuery query);
    Task<IResult<int>> GetRoomId(TryGetRoomIdCommand command);
}

public class AdminChatManager:IAdminChatManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public AdminChatManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/chat");
    }

    public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int? festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"history?festivalId={festivalId}&contactId={cId} "));
        var data = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
        return data;
    }

    public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync()
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"users"));
        var data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("save"), chatHistory);
        var data = await response.ToResult();
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
}