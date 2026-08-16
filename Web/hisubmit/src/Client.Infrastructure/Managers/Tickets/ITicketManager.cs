using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.AddEditTickets;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.DeleteTicket;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetTicketById;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Tickets;

public interface ITicketManager:ITransientManager
{
    Task<IResult> SaveTicketAsync(AddEditTicketsCommand command);
    Task<IResult> DeleteAsync(DeleteTicketCommand command);
    Task<PaginatedResult<GetAllTicketResponse>> GetAllAsync(GetAllTicketQuery query);
    Task<IResult<GetTicketByIdResponse>> GetDetailAsync(GetTicketByIdQuery query);
}

public class TicketManager : ITicketManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public TicketManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/ticket");
    }
    public async Task<IResult> SaveTicketAsync(AddEditTicketsCommand command)
    {
        var response =await _httpClient.PostAsJsonAsync(_baseEndPoint.GenerateUrl($"{command.FestivalId}/save"), command);
        return  await response.ToResult();
    }

    public async Task<IResult> DeleteAsync(DeleteTicketCommand command)
    {
        var response = await _httpClient.DeleteAsync(_baseEndPoint.GenerateUrl($"{command.FestivalId}/Delete", command));
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllTicketResponse>> GetAllAsync(GetAllTicketQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/GetAll", query));
        return await response.ToPaginatedResult<GetAllTicketResponse>();
    }

    public async Task<IResult<GetTicketByIdResponse>> GetDetailAsync(GetTicketByIdQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/Detail", query));
        return await response.ToResult<GetTicketByIdResponse>();
    }
}
