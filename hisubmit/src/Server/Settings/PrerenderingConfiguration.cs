using System.Linq;
using HiSubmit.Client.Infrastructure.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace HiSubmit.Server.Settings;

public static class PrerenderingConfiguration
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        var transientManager = typeof(ITransientManager);

        var transientTypes = transientManager
            .Assembly
            .GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Implementation = t,
                Service = t.GetInterface($"I{t.Name}")
            })
            .Where(t => t.Service != null);
            

        foreach (var type in transientTypes)
            if (transientManager.IsAssignableFrom(type.Service))
                services.AddTransient(type.Service, type.Implementation);
        
        return services;
    }
}