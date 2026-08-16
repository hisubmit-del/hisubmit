using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.FooterItems;
using Hisubmit.Client.SharedModels.Features.FooterItems.Commands;
using Hisubmit.Client.SharedModels.Features.FooterItems.Queries.GetAll;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.Footer;

public interface IFooterManager:ITransientManager
{
    Task<IResult<List<FooterItemDto>>> GetAllAsync(GetAllFooterItemQuery query);
    Task<IResult> SaveAsync(AddEditFooterItemCommand command);
    Task<IResult> DeleteAsync(DeleteFooterItemCommand command);
}

public class FooterManager : IFooterManager
{
    private readonly HttpClient _httpClient;
    private readonly BaseEndPoint _endPoint;

    public FooterManager(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _endPoint = new BaseEndPoint("api/v1/admin/footerItem");
    }
    public async Task<IResult<List<FooterItemDto>>> GetAllAsync(GetAllFooterItemQuery query)
    {
        var response = await _httpClient.GetAsync(_endPoint.GenerateUrl("getAll", query));
        return await response.ToResult<List<FooterItemDto>>();
    }

    public async Task<IResult> SaveAsync(AddEditFooterItemCommand command)
    {
        var response = await _httpClient.PostAsJsonAsync(_endPoint.GenerateUrl("save"), command);
        return await response.ToResult();
    }

    public async Task<IResult> DeleteAsync(DeleteFooterItemCommand command)
    {
        var response = await _httpClient.DeleteAsync(_endPoint.GenerateUrl("delete", command));
        return await response.ToResult();
    }
}