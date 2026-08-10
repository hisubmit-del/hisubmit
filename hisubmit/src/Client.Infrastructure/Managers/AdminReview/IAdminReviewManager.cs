using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Hisubmit.Client.SharedModels.Features.Reviews.Queries;
using HiSubmit.Client.Infrastructure.Extensions;
using HiSubmit.Client.Infrastructure.Routes;
using HiSubmit.Client.SharedModels.Wrapper;

namespace HiSubmit.Client.Infrastructure.Managers.AdminReview;

public interface IAdminReviewManager:ITransientManager
{
    Task<PaginatedResult<GetAllReviewResponse>> GetAll(GetAllReviewQuery query);
}

public class AdminReviewManager (HttpClient httpClient): IAdminReviewManager
{
    private readonly BaseEndPoint _baseEndPoint=new("api/v1/Admin/Review");
  
    public async Task<PaginatedResult<GetAllReviewResponse>> GetAll(GetAllReviewQuery query)
    {
        var response = await httpClient.PostAsJsonAsync
            (_baseEndPoint.GenerateUrl("GetAll"), query);
        return await response.ToPaginatedResult<GetAllReviewResponse>();
    }
}

