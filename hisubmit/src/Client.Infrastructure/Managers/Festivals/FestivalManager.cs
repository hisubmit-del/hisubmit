using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEdiitEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditAdditinalSettings;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalContact;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalDeadlines;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.CreateFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteDeadLineEntry;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteEventOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.DeleteVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddEditFestivalImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.AddFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.ReleaseFestival;
using Hisubmit.Client.SharedModels.Features.Festivals.Commands.SpecialRequest;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalPeriods;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetFestivalNames;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;

namespace HiSubmit.Client.Infrastructure.Managers.Festivals;

public class FestivalManager : IFestivalManager
{
    private readonly HttpClient _httpClient;
    public FestivalManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IResult<List<GetAllEventOrganizerResponse>>> GetAllOrganizerAsync(GetAllOrganizerQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetAllEventOrganizer(query));
        return await response.ToResult<List<GetAllEventOrganizerResponse>>();
    }

    public async Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync(GetFestivalDetailByIdQuery query)
    {        
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetFestivalById(query));
        return await response.ToResult<GetFestivalDetailResponse>();
    }

    public async Task<IResult> SpecialRequest(SpecialRequestCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync (FestivalEndPoint.SpecialRequest(command), command);
        return await response.ToResult();
    }

    public async Task<IResult<int>> SaveDetailAsync(AddEditFestivalDetailCommand request)
    {
        var response = await _httpClient.PostAsJsonAsync (FestivalEndPoint.UpdateDetail(request), request);
        return await response.ToResult<int>();
    }

    public async Task<IResult<int>> SaveOrginizerAsync(AddEditEventOrginizerCommand request)
    {
        var response = await _httpClient.PostAsJsonAsync (FestivalEndPoint.AddEditEventOrganizer(request), request);
        return await response.ToResult<int>();
    }

    public async Task<IResult<int>> DeleteOrginizer(DeleteEventOrginizerCommand command)
    {
        var response = await _httpClient.DeleteAsync(FestivalEndPoint.DeleteOrganizer(command));
        return await response.ToResult<int>();
    }

    public async Task<IResult<int>> SaveContactAsync(AddEditFestivalContactCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync (FestivalEndPoint.UpdateContact(command), command);
        return await response.ToResult<int>();
    }

    public async Task<IResult<int?>> SaveVenueAsync(AddEditFestivalVenueCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.AddEditVenue(command), command);
        return await response.ToResult<int?>();
    }

    public async Task<PaginatedResult<GetAllVenueResponse>> GetAllVenueAsync(GetAllVenueQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetAllVenue(query));
        return await response.ToPaginatedResult<GetAllVenueResponse>();
    }

    public async Task<IResult<int>> DeleteVenueAsync(DeleteVenueCommand command)
    {
        var response = await _httpClient.DeleteAsync(FestivalEndPoint.DeleteVenue(command));
        return await response.ToResult<int>();
    }

    public async Task<IResult<GetVenueByIdResponse>> GetVenueById(GetVenueByIdQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetVenueById(query));
        return await response.ToResult<GetVenueByIdResponse>();
    }

    public async Task<IResult<int>> SaveDeadLineAsync(AddEditFestivalDeadlineCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.UpdateDeadLine(command), command);
        return await response.ToResult<int>();
    }

    public async  Task<IResult<GetDeadLineByIdResponse>> AddEditDeadLineEntry(AddEditDeadLineEntryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.AddEditDeadlineEntry(request), request);
        return await response.ToResult<GetDeadLineByIdResponse>();
    }
    public async  Task<IResult<GetDeadLineByIdResponse>> GetDeadlineEntryDetail(GetDeadLineByIdQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetDeadlineEntryById(query));
        return await response.ToResult<GetDeadLineByIdResponse>();
    }
    public async  Task<IResult<List<GetAllDeadLineResponse>>> GetAllDeadlineEntry(GetAllDeadlineQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetAllDeadlineEntry(query));
        return await response.ToResult<List<GetAllDeadLineResponse>>();
    }
    public async Task<IResult<int>> DeleteDeadLineEntry(DeleteDeadLineEntryCommand command)
    {
        var response = await _httpClient.DeleteAsync(FestivalEndPoint.DeleteDeadLineEntry(command));
        return await response.ToResult<int>();
    }
    public async Task<IResult<int>> SaveAdditionalSetting(AddEditAdditionalSettingCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.UpdateAdditionalSettings(command), command);
        return await response.ToResult<int>();
    }
    public async Task<IResult<int>> AddFestival(AddFestivalCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.AddFestival(command), command);
        return await response.ToResult<int>();
    }
    public async Task<IResult> Release(ReleaseFestivalCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.ReleaseFestival(command), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetFestivalNamesResponse>> GetFestivalNames(GetFestivalNamesQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetFestivalsNames(query));
        return await response.ToPaginatedResult<GetFestivalNamesResponse>();
    }

    public async Task<IResult> UploadImages(AddEditFestivalImageCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.UploadImages(command), command);
        return await response.ToResult();
    }

    public async Task<PaginatedResult<GetAllFestivalImageResponse>> GetAllImages(GetAllFestivalImageQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.GetAllImages(query));
        return await response.ToPaginatedResult<GetAllFestivalImageResponse>();
    }

    public async Task<IResult<GetAllFestivalPeriodsResponse>> GetAllFestivalPeriods(GetAllFestivalPeriodsQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.AllPeriods(query));
        return await response.ToResult<GetAllFestivalPeriodsResponse>();
    }

    public async Task<PaginatedResult<GetAllReviewResponse>> GetAllReview(GetAllReviewQuery query)
    {
        var response = await _httpClient.GetAsync(FestivalEndPoint.AllReview(query));
        return await response.ToPaginatedResult<GetAllReviewResponse>();
    }

    public async Task<IResult> AddReview(AddReviewCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(FestivalEndPoint.AddReview(command), command);
        return await response.ToResult();
    }
}
