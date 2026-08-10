using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllShowHall;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Venues;

public interface IVenueManager:ITransientManager
{
    Task<IResult> SaveShowHall(AddEditShowHallCommand command);
    Task<PaginatedResult<GetAllVenueResponse>> GetAllVenue(GetAllVenueQuery query);
    Task<IResult<List<GetAllShowHallResponse>>>GetAllShowHalls(GetAllShowHallQuery query);
    Task<IResult<GetVenueByIdResponse>> GetVenueDetail(GetVenueByIdQuery query);
}

public class VenueManager : IVenueManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _baseEndPoint;

    public VenueManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseEndPoint = new BaseEndPoint("api/v1/venue");
    }
    public async Task<IResult> SaveShowHall(AddEditShowHallCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_baseEndPoint.GenerateUrl("saveShowHall"), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllVenueResponse>> GetAllVenue(GetAllVenueQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("GetAll", query));
        return await response.ToPaginatedResult<GetAllVenueResponse>();
    }

    public async Task<IResult<List<GetAllShowHallResponse>>> GetAllShowHalls(GetAllShowHallQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("GetAllShowHall",query));
        return await response.ToResult<List<GetAllShowHallResponse>>();
    }
    
    public async Task<IResult<GetVenueByIdResponse>> GetVenueDetail(GetVenueByIdQuery query)
    {
        var response = await _httpClient.GetAsync(_baseEndPoint.GenerateUrl("VenueDetail", query));
        return await response.ToResult<GetVenueByIdResponse>();
    }
}
