using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Seo;
using Hisubmit.Client.SharedModels.Features.Seo.GetPAgeSeoTags;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.AdminSeo;

public interface IAdminSeoManager:ITransientManager
{
    Task<IResult> AddEditSeoTags(AddEditSeoTagRequest request);
    Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query);
}

public class AdminSeoManager(HttpClient httpClient) : IAdminSeoManager
{
    private readonly BaseEndPoint _endPoint = new("api/v1/admin/seo");

    public async Task<IResult> AddEditSeoTags(AddEditSeoTagRequest request)
    {
        var res = await httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("SeoSetting"), request);
        return await res.ToResult();
    }
    
    public async Task<IResult<GetPageSeoTagResult>> GetPageSeoTag(GetPageSeoTagsQuery query)
    {
        var res = await httpClient.GetAsync(_endPoint.GenerateUrl("PageSeoTag", query));
        return await res.ToResult<GetPageSeoTagResult>();
    }
}