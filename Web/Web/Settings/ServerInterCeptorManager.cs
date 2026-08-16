using System.Threading.Tasks;
using HiSubmit.Client.Infrastructure.Managers.Interceptors;
using Toolbelt.Blazor;

public class ServerInterceptorManager:IHttpInterceptorManager
{
    public void RegisterEvent()
    {
        
    }

    public Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
    {
        return Task.CompletedTask;
    }

    public void DisposeEvent()
    {
        
    }
}