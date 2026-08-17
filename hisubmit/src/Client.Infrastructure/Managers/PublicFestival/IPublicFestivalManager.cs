using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLine;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllDeadLineEventCategory;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllOrginizer;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllVenue;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDeadLineById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetDetailById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetVenueById;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.AdminFestival.Queries.GetAllFestival;
using Hisubmit.Client.SharedModels.Features.Brands.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.FestivalFocs.Queries.GetAllFestivalFocus;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllImages;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.Products.Queries.GetAllPaged;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using Hisubmit.Client.SharedModels.Features.Reviews.Commands;
using HiSubmit.Client.Infrastructure.Routes.Festivals;
using Hisubmit.Client.SharedModels.Features.FestivalQualifyers.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.Products.Commands.AddEdit;
using Hisubmit.Hisubmit.Client.SharedModels.Features;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Products.Queries.GetById;
using Hisubmit.Client.SharedModels.Features.Festivals.Queries.GetAllFestivalFile;

namespace HiSubmit.Client.Infrastructure.Managers.PublicFestival;

public interface IPublicFestivalManager:ITransientManager
{
    Task<PaginatedResult<GetAllFestivalResponse>> GetAllFestival(GetAllFestivalRequest request);
    Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync(GetFestivalDetailByIdQuery query);
    Task<IResult<List<GetAllEventOrganizerResponse>>> GetAllOrganizerAsync(GetAllOrganizerQuery query);
    Task<IResult<List<GetAllVenueResponse>>> GetAllVenueAsync(GetAllVenueQuery query);
    Task<IResult<GetVenueByIdResponse>> GetVenueById(GetVenueByIdQuery query);
    Task<IResult<GetDeadLineByIdResponse>> GetDeadlineEntryDetail(GetDeadLineByIdQuery query);
    Task<IResult<List<GetAllDeadLineResponse>>> GetAllDeadlineEntry(GetAllDeadlineQuery query);
    Task<PaginatedResult<GetAllFestivalImageResponse>> GetAllImages(GetAllFestivalImageQuery query);
    Task<IResult<List<GetAllDeadLineEventCategoryResponse>>> GetAllGetDeadLineCategory(GetAllDeadLineEventCategoryQuery query);
    Task<PaginatedResult<GetAllReviewResponse>> GetAllReviews(GetAllReviewQuery query);
    Task<IResult<GetFestivalRatingSummaryResponse>> GetFestivalRatingSummary(GetFestivalRatingSummaryQuery query);
    Task<IResult> AddReview(AddReviewCommand command);
    Task<PaginatedResult<GetAllNewResponse>> GetAllNewsAsync(GetAllNewRequest request);
    Task<PaginatedResult<GetAllPagedProductsResponse>> GetAllProducts(GetAllProductsRequest request);
    Task<IResult<AddEditProductRequest>> GetProductById(GetProductByIdRequest request); 
    Task<IResult<List<GetAllArtCategoryResponse>>> GetAllArtCategories(GetAllArtCategoryRequest request);
    Task<IResult<List<GetAllFestivalFocusResponse>>> GetAllFestivalFocuses(GetAllFestivalFocusQuery query);
    Task<IResult<List<GetAllFestivalQualifiersResponse>>> GetAllFestivalQualifires(GetAllFestivalQualifiersQuery query);
    Task<IResult<int>> GetLikeCount(BaseFestivalRequest request);
    Task<IResult<bool>> GetLikeState(BaseFestivalRequest request);
    Task<IResult> AddDeleteLike(BaseFestivalRequest request);

    Task<IResult<List<GetAllFestivalFileResponse>>> GetAllFestivalFiles(GetAllFestivalFileQuery query);

}

public class PublicFestivalManager(HttpClient httpClient) : IPublicFestivalManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/public/festival");

    public async Task<IResult<List<GetAllDeadLineResponse>>> GetAllDeadlineEntry(GetAllDeadlineQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllDeadLineEntry", query));
        return await response.ToResult<List<GetAllDeadLineResponse>>();
    }

    public async Task<IResult<List<GetAllDeadLineEventCategoryResponse>>> GetAllGetDeadLineCategory(GetAllDeadLineEventCategoryQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllDeadlineEventCategory", query));
        return await response.ToResult<List<GetAllDeadLineEventCategoryResponse>>();
    }

    public async Task<PaginatedResult<GetAllReviewResponse>> GetAllReviews(GetAllReviewQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllReviews", query));
        return await response.ToPaginatedResult<GetAllReviewResponse>();
    }

    public async Task<IResult<GetFestivalRatingSummaryResponse>> GetFestivalRatingSummary(
        GetFestivalRatingSummaryQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("RatingSummary", query));
        return await response.ToResult<GetFestivalRatingSummaryResponse>();
    }

    public async Task<IResult> AddReview(AddReviewCommand command)
    {
        var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("Review"), command);
        return await response.ToResult();
    }

    public async Task<IResult<List<GetAllEventOrganizerResponse>>> GetAllOrganizerAsync(GetAllOrganizerQuery query)
    {

        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetAllOrganizer", query));
        return await response.ToResult<List<GetAllEventOrganizerResponse>>();
    }
    public async Task<PaginatedResult<GetAllFestivalImageResponse>> GetAllImages(GetAllFestivalImageQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Images",query));
        return await response.ToPaginatedResult<GetAllFestivalImageResponse>();
    }

    public async Task<IResult<List<GetAllVenueResponse>>> GetAllVenueAsync(GetAllVenueQuery query)
    {

        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetAllVenue", query));
        return await response.ToResult<List<GetAllVenueResponse>>();
    }

    public async Task<IResult<GetDeadLineByIdResponse>> GetDeadlineEntryDetail(GetDeadLineByIdQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("DetailDeadLine", query));
        return await response.ToResult<GetDeadLineByIdResponse>();
    }

    public async Task<PaginatedResult<GetAllFestivalResponse>> GetAllFestival(GetAllFestivalRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("getAll"),request);
        return await response.ToPaginatedResult<GetAllFestivalResponse>();
    }

    public async Task<IResult<GetFestivalDetailResponse>> GetFestivalDetailAsync(GetFestivalDetailByIdQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("getById", query));
        return await response.ToResult<GetFestivalDetailResponse>();
    }

    public async Task<IResult<GetVenueByIdResponse>> GetVenueById(GetVenueByIdQuery query)
    {

        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("VenueDetail", query));
        return await response.ToResult<GetVenueByIdResponse>();
    }
    public async Task<PaginatedResult<GetAllNewResponse>> GetAllNewsAsync(GetAllNewRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("GetAllNews",request));
        return await response.ToPaginatedResult<GetAllNewResponse>();
    }

    public async Task<PaginatedResult<GetAllPagedProductsResponse>> GetAllProducts(GetAllProductsRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllProducts", request));
        return await response.ToPaginatedResult<GetAllPagedProductsResponse>();
    }

    public async Task<IResult<AddEditProductRequest>> GetProductById(GetProductByIdRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Product", request));
        return await response.ToResult<AddEditProductRequest>();
    }

    public async Task<IResult<List<GetAllArtCategoryResponse>>> GetAllArtCategories(GetAllArtCategoryRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllArtCategory", request));
        return await response.ToResult<List<GetAllArtCategoryResponse>>();
    }

    public async Task<IResult<List<GetAllFestivalFocusResponse>>> GetAllFestivalFocuses(GetAllFestivalFocusQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("AllFestivalFocus", query));
        return await response.ToResult<List<GetAllFestivalFocusResponse>>();
    }

    public async Task<IResult<List<GetAllFestivalQualifiersResponse>>> GetAllFestivalQualifires(GetAllFestivalQualifiersQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Qualifiers", query));
        return await response.ToResult<List<GetAllFestivalQualifiersResponse>>();
    }

    public async Task<IResult<int>> GetLikeCount(BaseFestivalRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Likes", request));
        return await response.ToResult<int>();
    }

    public async Task<IResult<bool>> GetLikeState(BaseFestivalRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("LikeState", request));
        return await response.ToResult<bool>();
    }

    public async Task<IResult> AddDeleteLike(BaseFestivalRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("UpdateLike"), request);
        return await response.ToResult<bool>();
    }

    public async Task<IResult<List<GetAllFestivalFileResponse>>> GetAllFestivalFiles(GetAllFestivalFileQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("files", query));
        return await response.ToResult<List<GetAllFestivalFileResponse>>();
    }
    
}
