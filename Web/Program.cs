using BlazorApp4.Components.Account;
using Blazored.LocalStorage;
using Hangfire;
using Hangfire.Dashboard;
using HiSubmit.Application.Extensions;
using HiSubmit.Application.Filters;
using HiSubmit.Application.Interfaces.Carts;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.BackGroundJob;
using HiSubmit.Application.Services;
using HiSubmit.Client.Infrastructure.Managers.Identity.Authentication;
using HiSubmit.Client.Infrastructure.Managers.Interceptors;
using HiSubmit.Client.Infrastructure.Managers.Preferences;
using HiSubmit.Client.Infrastructure.Services;
using HiSubmit.Client.SharedModels.Constants.Application;
using HiSubmit.Client.SharedModels.Constants.Role;
using HiSubmit.Infrastructure.Extensions;
using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using MudBlazor;
using MudBlazor.Services;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Web.Components;
using Web.Components.Account;
using Web.Extensions;
using Web.Handlers;
using Web.Hubs;
using Web.Middlewares;
using Web.Services;
using Web.Settings;


var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddCors();

services.AddScoped<IBaseUrlService, BaseUrlService>();
services.AddSignalR();
services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
services.AddCurrentUserService();
services.AddSerialization();

services.AddDatabase(builder.Configuration);
services.AddServerStorage(); //TODO - should implement ServerStorageProvider to work correctly!

//services.AddScoped<ServerPreferenceManager>();
services.AddScoped<ICartService, CartService>();
services.AddScoped<IBackGroundJobService, BackGroundJobService>();
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<ISiteUrlService, SiteUrlService>();
services.AddScoped<ICheckPermission, CheckPermission>();
services.AddServerLocalization();

services.AddIdentity();
services.AddAuthorization();

//services.AddServerSideBlazor();

services.AddCookieAuthentication(builder.Environment);

services.AddScoped<IUserClaimsPrincipalFactory<BlazorHeroUser>, CustomClaimsPrincipalFactory>();

services.AddApplicationLayer();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddApplicationServices();
services.AddHttpContextAccessor();
services.AddRepositories();
services.AddExtendedAttributesUnitOfWork();
services.AddSharedInfrastructure(builder.Configuration);
services.AddInfrastructureMappings();


services.AddRecurringJobServices();
services.AddHangfireServer();
services.AddRazorViewRender();
services.AddPdfGenerator();
services.AddQrCodeGenerator();
services.AddControllers().AddValidators();
services.AddExtendedAttributesValidators();
services.AddExtendedAttributesHandlers();
services.AddSession();
services.AddRazorPages();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1 * 1024 * 1024; // 10MB
});
//    .AddApplicationPart(typeof(Web.Components.Pages.Festival.Festival).Assembly);
// Program.cs
//builder.Services.AddMvc().AddApplicationPart(typeof(Web.Components.Pages.Festival.Festival).Assembly);

services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

services.AddLazyCache();

services.AddHangfire(x =>
    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

services.AddTransient<CookieHandler>();
//HttpClientHandler clientHandler = new HttpClientHandler();
//clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

HttpClientHandler clientHandler = new HttpClientHandler();
clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

services.AddTransient<CookieHandler>(); // حتماً CookieHandler رو ثبت کن

services.AddSingleton<HttpClient>(sp =>
{
    var server = sp.GetRequiredService<IServer>();
    var addressFeature = server.Features.Get<IServerAddressesFeature>();
    var cookieHandler = sp.GetRequiredService<CookieHandler>();

    cookieHandler.InnerHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
    };

    string? baseAddress;

    if (addressFeature != null && addressFeature.Addresses.Any())
    {
        baseAddress = addressFeature.Addresses.FirstOrDefault(address =>
            Uri.IsWellFormedUriString(address, UriKind.Absolute));
    }
    else
    {
        baseAddress = "https://localhost:44343"; // fallback development address
    }

    //var  baseAddress = new Uri("http://localhost:8090");

    if (!Uri.TryCreate(baseAddress, UriKind.Absolute, out var baseUri))
    {
        throw new InvalidOperationException($"BaseAddress '{baseAddress}' is invalid.");
    }

    //var innerHandler = new CookieHandler()
    //{
    //    InnerHandler = clientHandler
    //};

    //baseUri =new Uri("http://localhost:8090");

    return new HttpClient(cookieHandler)
    {
        BaseAddress = baseUri
    };
});

//services.AddScoped<LazyAssemblyLoader>();
//Prerendering Config
//services.AddSingleton<HttpClient>(sp =>
//{
//    // Get the address that the app is currently running at
//    var server = sp.GetRequiredService<IServer>();
//    var addressFeature = server.Features.Get<IServerAddressesFeature>();
//    string baseAddress;
//    // var logger = sp.GetRequiredService<ILogger>();
//    //logger.Error(addressFeature.Addresses.First());
//    if (addressFeature.Addresses.First() != "http://*:8081/")
//    {
//        baseAddress = addressFeature.Addresses.Last();
//    }
//    else
//    {
//        baseAddress = "http://localhost:8081";
//    }

//    var ht = new HttpClient(clientHandler) { BaseAddress =new Uri(baseAddress) };
//    return ht
//        ;
//});


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

services.AddHttpContextAccessor();

services.AddManagers();

// --------------------------------------------------------Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

//builder.Services.AddAuthentication(options =>
//    {
//        options.DefaultScheme = IdentityConstants.ApplicationScheme;
//        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//    })
//    .AddIdentityCookies();

builder.Host.UseSerilog();
var app = builder.Build();

app.UseCors();
app.UseExceptionHandling(app.Environment);

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseMigrationsEndPoint();
//}
//else
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlerMiddleware>();
//app.UseBlazorFrameworkFiles();
var filesPath = Path.Combine(app.Environment.ContentRootPath, "Files");
if (!Directory.Exists(filesPath))
{
    Directory.CreateDirectory(filesPath);
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(filesPath),
    RequestPath = new PathString("/Files")
});

app.UseRequestLocalizationByCulture();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CheckLogoutUser>();
app.UseHangfireDashboard("/jobs", new DashboardOptions
{
    DashboardTitle = "HiSubmit Jobs",
    //Authorization = new[] { new HangfireAuthorizationFilter() },
    IsReadOnlyFunc = (context => context.GetHttpContext().User
        .IsInRole(RoleConstants.AdministratorRole))
});

//app.UseEndpoints((endpoints) =>
//{
    //endpoints.MapRazorPages();
    //endpoints.MapControllers();
    // endpoints.MapFallbackToFile("index.html");
    //endpoints.MapBlazorHub();
    //endpoints.MapFallbackToPage("/_Host");
    // endpoints.MapGet("/notFound", async context =>
    // {
    //     //context.Response.StatusCode = 404;
    //     
    //     //await context.Response.WriteAsync("404 not found");
    // });
   // endpoints.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);
//});

app.ConfigureSwagger();
app.Initialize(builder.Configuration);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapControllers();
app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
