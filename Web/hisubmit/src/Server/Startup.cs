using BlazorApp4.Components.Account;
using Blazored.LocalStorage;
using Hangfire;
using Hangfire.Dashboard;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Jobs.Daily.Festivals;
using HiSubmit.Application.Services;
using HiSubmit.Client.Infrastructure.Authentication;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.Infrastructure.Managers.Interceptors;
using HiSubmit.Client.Infrastructure.Managers.Preferences;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.Services;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Infrastructure.Extensions;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Infrastructure.Validators;
using HiSubmit.Server.Extensions;
using HiSubmit.Server.Filters;
using HiSubmit.Server.Hubs;
using HiSubmit.Server.Managers.Preferences;
using HiSubmit.Server.Middlewares;
using HiSubmit.Server.Services;
using HiSubmit.Server.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Services;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace HiSubmit.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

    }

    private readonly IConfiguration _configuration;
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddCors();
      
        services.AddSignalR();
        services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
        services.AddCurrentUserService();
        services.AddSerialization();
        services.AddDatabase(_configuration);
        services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!
        services.AddScoped<ServerPreferenceManager>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IBackGroundJobService, BackGroundJobService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISiteUrlService, SiteUrlService>();
        services.AddScoped<ICheckPermission, CheckPermission>();
        services.AddServerLocalization();

        services.AddIdentity();
        services.AddAuthorization();
        //services.AddIdentityCore<BlazorHeroUser>()
        //    .AddRoles<BlazorHeroRole>()
        //    .AddRoleValidator<FestivalRoleValidator>();

        services.AddServerSideBlazor();
        services.AddCookieAuthentication();

        services.AddApplicationLayer();
        services.AddApplicationServices();
        services.AddHttpContextAccessor();
        services.AddRepositories();
        services.AddExtendedAttributesUnitOfWork();
        services.AddSharedInfrastructure(_configuration);
        services.RegisterSwagger();
        services.AddInfrastructureMappings();
        services.AddHangfire(x =>
            x.UseSqlServerStorage(_configuration.GetConnectionString("DefaultConnection")));

        services.AddRecurringJobServices();
        services.AddHangfireServer();
        services.AddRazorViewRender();
        services.AddPdfGenerator();
        services.AddQrCodeGenerator();
        services.AddControllers().AddValidators();
        services.AddExtendedAttributesValidators();
        services.AddExtendedAttributesHandlers();
        services.AddRazorPages();
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
        services.AddLazyCache();



        HttpClientHandler clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
        //services.AddScoped<LazyAssemblyLoader>();
        //Prerendering Config
        services.AddSingleton<HttpClient>(sp =>
        {
            // Get the address that the app is currently running at
            var server = sp.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            string baseAddress;
            // var logger = sp.GetRequiredService<ILogger>();
            //logger.Error(addressFeature.Addresses.First());

            if (addressFeature.Addresses.First() != "http://*:8081/")
            {
                baseAddress = addressFeature.Addresses.Last();
            }
            else
            {
                baseAddress = "http://localhost:8081";
            }

            var ht = new HttpClient(clientHandler) { BaseAddress =new Uri(baseAddress )};
            return ht
                ;
        });

        services.AddMudServices(configuration =>
        {
            configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            configuration.SnackbarConfiguration.HideTransitionDuration = 100;
            configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
            configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
            configuration.SnackbarConfiguration.ShowCloseIcon = false;
        });
        services.AddHttpClientInterceptor();
        services.AddScoped<UserCartService>();
        services.AddScoped<UserNotificationService>();
        services.AddScoped<SelectedAccountService>();
        services.AddScoped<MainLayoutService>();
        services.AddScoped<IAuthenticationManager, ServerAuthenticationManager>();
        services.AddScoped<ILocalStorageService, ServerLocalStorageService>();
        services.AddScoped<IHttpInterceptorManager, ServerInterceptorManager>();
        services.AddScoped<ClientPreferenceManager>();
        services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
        //services.AddScoped<HiSubmitAuthenticationStateProvider>();
        //services.AddScoped<AuthenticationStateProvider, HiSubmitAuthenticationStateProvider>();
        services.AddScoped<ScrollService>();
        services.AddScoped<GalleryImagesOverlayService>();
        services.AddManagers();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
        IStringLocalizer<Startup> localizer, IGoToNextPeriodOfFestival goToNextPeriodOfFestival)
    {


        //goToNextPeriodOfFestival.InvokeAsync();

        //var options = new RewriteOptions().AddRedirectToWww(); ;
        //app.UseRewriter(options);
        app.UseCors();
        app.UseExceptionHandling(env);
        app.UseHttpsRedirection();
        app.UseMiddleware<ErrorHandlerMiddleware>();
        //app.UseBlazorFrameworkFiles();
        app.UseStaticFiles();
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });

        app.UseRequestLocalizationByCulture();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            DashboardTitle = localizer["Hisubmit Jobs"],
            Authorization = new[] { new HangfireAuthorizationFilter() },
            IsReadOnlyFunc = (context => context.GetHttpContext().User
                .IsInRole(RoleConstants.AdministratorRole))
        });
        app.UseEndpoints((endpoints) =>
        {
            endpoints.MapRazorPages();
            endpoints.MapControllers();
            // endpoints.MapFallbackToFile("index.html");
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            // endpoints.MapGet("/notFound", async context =>
            // {
            //     //context.Response.StatusCode = 404;
            //     
            //     //await context.Response.WriteAsync("404 not found");
            // });
            endpoints.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);
        });

        app.ConfigureSwagger();
        app.Initialize(_configuration);
    }
}