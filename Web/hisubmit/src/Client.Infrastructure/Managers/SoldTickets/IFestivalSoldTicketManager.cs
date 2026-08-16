using System.Net.Http;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.SoldTickets.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.SoldTickets;

public interface IFestivalSoldTicketManager:ITransientManager
{
    Task<PaginatedResult<GetAllSoldTicketResponse>> GetAllSoldTicket(GetAllSoldTicketQuery query);
}

public class FestivalSoldTicketManager : IFestivalSoldTicketManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public FestivalSoldTicketManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/soldTicket");
    }

    public async Task<PaginatedResult<GetAllSoldTicketResponse>> GetAllSoldTicket(GetAllSoldTicketQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl($"{query.FestivalId}/GetAll", query));
        return await response.ToPaginatedResult<GetAllSoldTicketResponse>();
    }
}
