using System.Threading.Tasks;
using Toolbelt.Blazor;

namespace HiSubmit.Client.Infrastructure.Managers.Interceptors
{
    public interface IHttpInterceptorManager : ITransientManager
    {
        void RegisterEvent();

        Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e);

        void DisposeEvent();
    }
}