using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Tickets.Queries.GetAllTicket;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.PublicTicket;

public interface IPublicTicketManager:ITransientManager
{
    Task<PaginatedResult<GetAllTicketResponse>> GetAllAsync(GetAllTicketQuery query);
}

public class PublicTicketManager : IPublicTicketManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public PublicTicketManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/public/ticket");
    }
    public async Task<PaginatedResult<GetAllTicketResponse>> GetAllAsync(GetAllTicketQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("GetAll", query));
        return await response.ToPaginatedResult<GetAllTicketResponse>();
    }
}
