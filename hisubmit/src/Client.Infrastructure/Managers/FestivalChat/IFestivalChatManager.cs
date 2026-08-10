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

namespace HiSubmit.Client.Infrastructure.Managers.FestivalChat;

public interface IFestivalChatManager:ITransientManager
{
    Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(int festivalId);
    Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory,int festivalId);
    Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int festivalId);
    Task<IResult<List<GetAllRoomResponse>>> GetAllRooms(GetAllRoomQuery query,int festivalId);
    Task<IResult> AddMessage(AddChatMessageRequest message,int festivalId);
    Task<IResult<List<GetChatHistoryResponse>>> GetAllChatMessage(GetChatHistoryQuery query,int festivalId);
    Task<IResult<List<GetAllContactResponse>>> GetAllContact(GetAllContactQuery query,int festivalId);
    Task<IResult<int>> GetRoomId(TryGetRoomIdCommand command,int festivalId);
}


public class FestivalChatManager:IFestivalChatManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public FestivalChatManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/festivalChat");
    }

    public async Task<IResult<IEnumerable<ChatHistoryResponse>>> GetChatHistoryAsync(string cId,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/{cId}"));
        var data = await response.ToResult<IEnumerable<ChatHistoryResponse>>();
        return data;
    }

    public async Task<IResult<IEnumerable<ChatUserResponse>>> GetChatUsersAsync(int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/users"));
        var data = await response.ToResult<IEnumerable<ChatUserResponse>>();
        return data;
    }

    public async Task<IResult> SaveMessageAsync(ChatHistory<IChatUser> chatHistory,int festivalId)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}"), chatHistory);
        var data = await response.ToResult();
        return data;
    }
    
    public async Task<IResult<List<GetAllRoomResponse>>> GetAllRooms(GetAllRoomQuery query,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/rooms", query));
        return await response.ToResult<List<GetAllRoomResponse>>();
    }

    public async Task<IResult> AddMessage(AddChatMessageRequest message,int festivalId)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/AddMessage"),message);
        return await response.ToResult();
    }

    public async Task<IResult<List<GetChatHistoryResponse>>> GetAllChatMessage(GetChatHistoryQuery query,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/ChatMessages", query));
        return await response.ToResult<List<GetChatHistoryResponse>>();
    }

    public async Task<IResult<List<GetAllContactResponse>>> GetAllContact(GetAllContactQuery query,int festivalId)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl($"{festivalId}/contacts", query));
        return await response.ToResult<List<GetAllContactResponse>>();
    }

    public async Task<IResult<int>> GetRoomId(TryGetRoomIdCommand command,int festivalId)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl($"{festivalId}/GetRoomId"), command);
        return await response.ToResult<int>();
    }
}
