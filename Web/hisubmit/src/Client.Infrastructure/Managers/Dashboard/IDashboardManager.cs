using Hisubmit.Client.SharedModels.Features.Dashboards.Queries.GetData;
using HiSubmit.Client.SharedModels.Wrapper;
using System.Threading.Tasks;

namespace HiSubmit.Client.Infrastructure.Managers.Dashboard
{
    public interface IDashboardManager : ITransientManager
    {
        Task<IResult<DashboardDataResponse>> GetDataAsync();
    }
}