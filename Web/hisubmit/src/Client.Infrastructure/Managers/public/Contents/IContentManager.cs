using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using Hisubmit.Client.SharedModels.Features.News.Queries;
using Hisubmit.Client.SharedModels.Features.StaticPages.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;
using Hisubmit.Hisubmit.Client.SharedModels.Features.Likes;

namespace HiSubmit.Client.Infrastructure.Managers.Contents;

public interface IContentManager:ITransientManager
{
    Task<IResult<GetDetailNewResponse>> GetNewDetail(GetDetailNewQuery query);
    Task<PaginatedResult<GetAllNewResponse>> GetAllNew(GetAllNewRequest request);
    Task<IResult<List<FooterItemDto>>> GetFooterItems(GetAllFooterItemQuery query);
    Task<IResult<GetDetailStaticPageResponse>> GetStaticPage(GetDetailStaticPageQuery query);
    Task<IResult<int>> GetLikeCount(GetLikeCountRequest request);
    Task<IResult<bool>> GetLikeState(GetLikeCountRequest request);
    Task<IResult> AddDeleteLike(GetLikeCountRequest request);

    Task<PaginatedResult<GetAllStaticPageResponse>> GetAllFAQ(GetAllStaticPageRequest request);
}

public class ContentManager(HttpClient httpClient) : IContentManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/public/content");

    public async Task<PaginatedResult<GetAllNewResponse>> GetAllNew(GetAllNewRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("News",request));
        return await response.ToPaginatedResult<GetAllNewResponse>();
    }

    public async Task<IResult<List<FooterItemDto>>> GetFooterItems(GetAllFooterItemQuery query)
    {
        var f = httpClient.BaseAddress;
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("FooterItems", query));
        return await response.ToResult<List<FooterItemDto>>();
    }

    public async Task<IResult<GetDetailStaticPageResponse>> GetStaticPage(GetDetailStaticPageQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("staticPage", query));
        return await response.ToResult<GetDetailStaticPageResponse>();
    }

    public async Task<IResult<GetDetailNewResponse>> GetNewDetail(GetDetailNewQuery query)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("New",query));
        return await response.ToResult<GetDetailNewResponse>();
    }

    public async Task<IResult<int>> GetLikeCount(GetLikeCountRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("Likes", request));
        return await response.ToResult<int>();
    }
    public async Task<IResult<bool>> GetLikeState(GetLikeCountRequest request)
    {
        var response = await httpClient.GetAsync(_endPoint.GenerateUrl("LikeState", request));
        return await response.ToResult<bool>();
    }

    public async Task<IResult> AddDeleteLike(GetLikeCountRequest request)
    {
        var response = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("UpdateLike"), request);
        return await response.ToResult<bool>();
    }

    public async Task<PaginatedResult<GetAllStaticPageResponse>> GetAllFAQ(GetAllStaticPageRequest request)
    {
       var response= await httpClient.GetAsync(_endPoint.GenerateUrl("GetFAQ", request));
        return await response.ToPaginatedResult<GetAllStaticPageResponse>();
    }
}