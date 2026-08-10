//using Blazored.LocalStorage;
//using HiSubmit.Client.Infrastructure.Authentication;
//using HiSubmit.Client.Infrastructure.Managers;
//using HiSubmit.Client.Infrastructure.Managers.ExtendedAttribute;
//using HiSubmit.Client.Infrastructure.Managers.Preferences;
//using Hisubmit.Client.SharedModels.Contracts.Permission;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//using Microsoft.Extensions.DependencyInjection;
//using MudBlazor;
//using MudBlazor.Services;
//using System;
//using System.Globalization;
//using System.Linq;
//using System.Net.Http;
//using System.Reflection;
//using Hisubmit.Client.SharedModels.Entities.ExtendedAttributes;
//using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
//using HiSubmit.Client.Infrastructure.Managers.Interceptors;
//using HiSubmit.Client.Infrastructure.Services;
//using HiSubmit.Client.Services;
//using Hisubmit.Client.SharedModels.Entities.Misc;
//using Microsoft.AspNetCore.Components.WebAssembly.Services;
//using Microsoft.Extensions.Logging;
//using Toolbelt.Blazor.Extensions.DependencyInjection;

//namespace HiSubmit.Web.Extensions;

//public static class WebAssemblyHostBuilderExtensions
//{
//    private const string ClientName = "HisubmitIcon.API";

//    public static WebAssemblyHostBuilder SetLogger(this WebAssemblyHostBuilder builder)
//    {
//        builder.Logging.SetMinimumLevel(builder.HostEnvironment.IsEnvironment("Developing")
//            ? LogLevel.None
//            : LogLevel.Information);
//        return builder;
//    }
//    public static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
//    {
//        // builder.RootComponents.Add<App>("#app");

//        return builder;
//    }

//    public static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
//    {
//        builder
//            .Services
//            .AddLocalization(options => { options.ResourcesPath = "Resources"; })
//            .AddAuthorizationCore(RegisterPermissionClaims)
//            .AddBlazoredLocalStorage()
//            .AddMudServices(configuration =>
//            {
//                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
//                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
//                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
//                configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
//                configuration.SnackbarConfiguration.ShowCloseIcon = false;
//            })
//            .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
//            .AddScoped<ClientPreferenceManager>()
//            .AddScoped<HiSubmitAuthenticationStateProvider>()
//            .AddScoped<AuthenticationStateProvider, HiSubmitAuthenticationStateProvider>()
//            .AddManagers()
//            .AddExtendedAttributeManagers()
//            .AddTransient<AuthenticationHeaderHandler>()
//            .AddScoped(sp => sp
//                .GetRequiredService<IHttpClientFactory>()
//                .CreateClient(ClientName).EnableIntercept(sp))
//            .AddHttpClient(ClientName, client =>
//            {
//                client.DefaultRequestHeaders.AcceptLanguage.Clear();
//                client.DefaultRequestHeaders.AcceptLanguage.ParseAdd(CultureInfo.DefaultThreadCurrentCulture
//                    ?.TwoLetterISOLanguageName);
//                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
//            })
//            .AddHttpMessageHandler<AuthenticationHeaderHandler>();
//        builder.Services.AddScoped<IHttpInterceptorManager, HttpInterceptorTManager>();
//        builder.Services.AddScoped<IAuthenticationManager, ClientAuthenticationManager>();
//        builder.Services.AddHttpClientInterceptor();
//        builder.Services.AddScoped<UserCartService>();
//        builder.Services.AddScoped<UserNotificationService>();
//        builder.Services.AddScoped<SelectedAccountService>();
//        builder.Services.AddScoped<MainLayoutService>();
//        builder.Services.AddScoped<ScrollService>();
//        builder.Services.AddSingleton<LazyAssemblyLoader>();
//        builder.Services.AddSingleton<GalleryImagesOverlayService>();
//        builder.Services.AddVideoPlayerServices();
//        return builder;
//    }

//    private static IServiceCollection AddManagers(this IServiceCollection services)
//    {
//        var transientManager = typeof(ITransientManager);

//        var transientTypes = transientManager
//            .Assembly
//            .GetExportedTypes()
//            .Where(t => t.IsClass && !t.IsAbstract)
//            .Select(t => new
//            {
//                Implementation = t,
//                Service = t.GetInterface($"I{t.Name}")
//            })
//            .Where(t => t.Service != null);
            

//        foreach (var type in transientTypes)
//            if (transientManager.IsAssignableFrom(type.Service))
//                services.AddTransient(type.Service, type.Implementation);
        
//        return services;
//    }

//    private static IServiceCollection AddExtendedAttributeManagers(this IServiceCollection services)
//    {
//        return services
//            .AddTransient(typeof(IExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>), typeof(ExtendedAttributeManager<int, int, Document, DocumentExtendedAttribute>));
//    }

//    private static void RegisterPermissionClaims(AuthorizationOptions options)
//    {
//        foreach (var prop in typeof(Permissions).GetNestedTypes()
//                     .SelectMany(c => c.GetFields
//                         (BindingFlags.Public | BindingFlags.Static | 
//                          BindingFlags.FlattenHierarchy)))
//        {
//            var propertyValue = prop.GetValue(null);
//            if (propertyValue is not null)
//            {
//                options.AddPolicy(propertyValue.ToString()!, 
//                    policy => policy.RequireClaim(ApplicationClaimTypes.Permission,
//                        propertyValue.ToString()!));
//            }
//        }
//    }
//}
