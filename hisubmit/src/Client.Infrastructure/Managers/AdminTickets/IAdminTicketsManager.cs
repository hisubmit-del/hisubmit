using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HiSubmit.Client.SharedModels.Wrapper;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.Infrastructure.Extensions;
using Hisubmit.Client.SharedModels.Features.Tickets.Commands.Enable;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;

namespace HiSubmit.Client.Infrastructure.Managers.AdminTickets;

public interface IAdminTicketsManager:ITransientManager
{
    Task<PaginatedResult<GetAllTicketResponse>> GetAll(GetAllTicketQuery query);
    Task<IResult> EnableTickets(EnableTicketCommand request);
}

public  class  AdminTicketsManager:IAdminTicketsManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public AdminTicketsManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("/api/v1/admin/tickets");
    }
    public async Task<PaginatedResult<GetAllTicketResponse>> GetAll(GetAllTicketQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("GetAll", query));
        return await response.ToPaginatedResult<GetAllTicketResponse>();
    }

    public async Task<IResult> EnableTickets(EnableTicketCommand request)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("UpdateEnable"), request);
        return await response.ToResult();
    }
}