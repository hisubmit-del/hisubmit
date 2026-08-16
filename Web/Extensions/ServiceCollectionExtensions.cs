using DinkToPdf;
using DinkToPdf.Contracts;
using HiSubmit.Application.Configurations;
using HiSubmit.Application.Interfaces.GenerateQrCode;
using HiSubmit.Application.Interfaces.PdfConverter;
using HiSubmit.Application.Interfaces.RenderView;
using HiSubmit.Application.Interfaces.Serialization.Options;
using HiSubmit.Application.Interfaces.Serialization.Serializers;
using HiSubmit.Application.Interfaces.Serialization.Settings;
using HiSubmit.Application.Interfaces.Services;
using HiSubmit.Application.Interfaces.Services.Account;
using HiSubmit.Application.Interfaces.Services.Identity;
using HiSubmit.Application.Jobs.Daily.Festivals;
using HiSubmit.Application.Serialization.JsonConverters;
using HiSubmit.Application.Serialization.Options;
using HiSubmit.Application.Serialization.Serializers;
using HiSubmit.Application.Serialization.Settings;
using HiSubmit.Client.SharedModels.Constants.Localization;
using HiSubmit.Infrastructure;
using HiSubmit.Infrastructure.Contexts;
using HiSubmit.Infrastructure.Models.Identity;
using HiSubmit.Infrastructure.Services;
using HiSubmit.Infrastructure.Services.Identity;
using HiSubmit.Infrastructure.Shared.Services;
using Web.Managers.Preferences;
using Web.Permission;
using Web.Services;
using Web.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using System.Globalization;
using Web.Hubs;

namespace Web.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static async Task<IStringLocalizer> GetRegisteredServerLocalizerAsync<T>(this IServiceCollection services) where T : class
    {
        var serviceProvider = services.BuildServiceProvider();
        await SetCultureFromServerPreferenceAsync(serviceProvider);
        var localize = serviceProvider.GetService<IStringLocalizer<T>>();
        await serviceProvider.DisposeAsync();
        return localize;
    }

    private static async Task SetCultureFromServerPreferenceAsync(IServiceProvider serviceProvider)
    {
        var storageService = serviceProvider.GetService<ServerPreferenceManager>();
        if (storageService != null)
        {
            // TODO - should implement ServerStorageProvider to work correctly!
            CultureInfo culture;
            var preference = await storageService.GetPreference() as ServerPreference;
            if (preference != null)
                culture = new CultureInfo(preference.LanguageCode);
            else
                culture = new CultureInfo(LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }

    internal static IServiceCollection AddServerLocalization(this IServiceCollection services)
    {
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(SignalRHub));
        return services;
    }

    internal static AppConfiguration GetApplicationSettings(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var applicationSettingsConfiguration = configuration.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettingsConfiguration);
        return applicationSettingsConfiguration.Get<AppConfiguration>();
    }

    //internal static void RegisterSwagger(this IServiceCollection services)
    //{
    //    services.AddSwaggerGen(async c =>
    //    {
    //        //TODO - Lowercase Swagger Documents
    //        //c.DocumentFilter<LowercaseDocumentFilter>();
    //        //Refer - https://gist.github.com/rafalkasa/01d5e3b265e5aa075678e0adfd54e23f

    //        // include all _project's xml comments
    //        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    //        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
    //        {
    //            if (!assembly.IsDynamic)
    //            {
    //                var xmlFile = $"{assembly.GetName().Name}.xml";
    //                var xmlPath = Path.Combine(baseDirectory, xmlFile);
    //                if (File.Exists(xmlPath))
    //                {
    //                    c.IncludeXmlComments(xmlPath);
    //                }
    //            }
    //        }

    //        c.SwaggerDoc("v1", new OpenApiInfo
    //        {
    //            Version = "v1",
    //            Title = "HiSubmit",
    //            License = new OpenApiLicense
    //            {
    //                Name = "MIT License",
    //                Url = new Uri("https://opensource.org/licenses/MIT")
    //            }
    //        });

    //        var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

    //        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //        {
    //            Name = "Authorization",
    //            In = ParameterLocation.Header,
    //            Type = SecuritySchemeType.ApiKey,
    //            Scheme = "Bearer",
    //            BearerFormat = "JWT",
    //            Description = localizer["Input your Bearer token in this format - Bearer {your token here} to access this API"],
    //        });
    //        c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //        {
    //            {
    //                new OpenApiSecurityScheme
    //                {
    //                    Reference = new OpenApiReference
    //                    {
    //                        Type = ReferenceType.SecurityScheme,
    //                        Id = "Bearer",
    //                    },
    //                    Scheme = "Bearer",
    //                    Name = "Bearer",
    //                    In = ParameterLocation.Header,
    //                }, new List<string>()
    //            },
    //        });
    //    });
    //}

    internal static IServiceCollection AddSerialization(this IServiceCollection services)
    {
        services
            .AddScoped<IJsonSerializerOptions, SystemTextJsonOptions>()
            .Configure<SystemTextJsonOptions>(configureOptions =>
            {
                if (configureOptions.JsonSerializerOptions.Converters.All(c => c.GetType() != typeof(TimespanJsonConverter)))
                    configureOptions.JsonSerializerOptions.Converters.Add(new TimespanJsonConverter());
            });
        services.AddScoped<IJsonSerializerSettings, NewtonsoftJsonSettings>();

        services.AddScoped<IJsonSerializer, SystemTextJsonSerializer>(); // you can change it
        return services;
    }

    internal static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
        => services
            .AddDbContext<BlazorHeroContext>(options => options
                .UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
            .AddTransient<IDatabaseSeeder, DatabaseSeeder>();

    internal static IServiceCollection AddCurrentUserService(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    internal static IServiceCollection AddIdentity(this IServiceCollection services)
    {
        services
            .AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
            .AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>()
            .AddIdentity<BlazorHeroUser, BlazorHeroRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<BlazorHeroContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        return services;
    }

    internal static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IDateTimeService, SystemDateTimeService>();
        services.Configure<MailConfiguration>(configuration.GetSection("MailConfiguration"));
        services.Configure<SiteURLConfiguration>(configuration.GetSection("SiteURLConfiguration"));
        services.AddTransient<IMailService, SMTPMailService>();
        return services;
    }

    internal static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IRoleClaimService, RoleClaimService>();
        services.AddTransient<ITokenService, IdentityService>();
        services.AddTransient<IRoleService, RoleService>();
        services.AddTransient<IAccountService, AccountService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IChatService, ChatService>();
        services.AddTransient<IUploadService, UploadService>();
        services.AddTransient<IAuditService, AuditService>();
        services.AddScoped<IExcelService, ExcelService>();
        return services;
    }


    public static IServiceCollection AddCookieAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment environment)
    {
        services
            .AddAuthentication(IdentityConstants.ApplicationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/account/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/account/access-denied";
                options.Cookie.Name = "HiSubmit.Auth";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/api"))
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            return Task.CompletedTask;
                        }

                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.HttpOnly = true;

            options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest; // یا Always
            options.Cookie.SameSite = SameSiteMode.Lax; // یا None برای cross-domain

            options.ExpireTimeSpan = TimeSpan.FromDays(2);
            options.SlidingExpiration = true; // تمدید خودکار با فعالیت کاربر
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = environment.IsDevelopment()
                ? CookieSecurePolicy.SameAsRequest
                : CookieSecurePolicy.Always;
        });

        //builder.Services.AddAuthentication(options =>
        //    {
        //        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        //        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        //    })
        //    .AddIdentityCookies();
        //services.AddAuthorization(options =>
        //{
        //    foreach (var prop in typeof(Permissions)
        //                 .GetNestedTypes()
        //                 .SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        //    {
        //        var value = prop.GetValue(null)?.ToString();
        //        if (!string.IsNullOrWhiteSpace(value))
        //        {
        //            options.AddPolicy(value, policy =>
        //                policy.RequireClaim(ApplicationClaimTypes.Permission, value));
        //        }
        //    }
        //});

        return services;
    }


    //internal static IServiceCollection AddJwtAuthentication(
    //    this IServiceCollection services, AppConfiguration config)
    //{
    //    var key = Encoding.ASCII.GetBytes(config.Secret);
    //    services
    //        .AddAuthentication(authentication =>
    //        {
    //            authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //            authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //        })
    //        .AddJwtBearer(async bearer =>
    //        {
    //            bearer.RequireHttpsMetadata = false;
    //            bearer.SaveToken = true;
    //            bearer.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                IssuerSigningKey = new SymmetricSecurityKey(key),
    //                ValidateIssuer = false,
    //                ValidateAudience = false,
    //                RoleClaimType = ClaimTypes.Role,
    //                ClockSkew = TimeSpan.Zero
    //            };

    //            var localizer = await GetRegisteredServerLocalizerAsync<ServerCommonResources>(services);

    //            bearer.Events = new JwtBearerEvents
    //            {
    //                OnAuthenticationFailed = c =>
    //                {
    //                    if (c.Exception is SecurityTokenExpiredException)
    //                    {
    //                        c.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //                        c.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(Result.Fail(localizer["The Token is expired."]));
    //                        return c.Response.WriteAsync(result);
    //                    }
    //                    else
    //                    {
    //                        c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
    //                        c.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(Result.Fail(localizer["An unhandled error has occurred."]));
    //                        return c.Response.WriteAsync(result);
    //                    }
    //                },
    //                OnChallenge = context =>
    //                {
    //                    context.HandleResponse();
    //                    if (!context.Response.HasStarted)
    //                    {
    //                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
    //                        context.Response.ContentType = "application/json";
    //                        var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not Authorized."]));
    //                        return context.Response.WriteAsync(result);
    //                    }

    //                    return Task.CompletedTask;
    //                },
    //                OnForbidden = context =>
    //                {
    //                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
    //                    context.Response.ContentType = "application/json";
    //                    var result = JsonConvert.SerializeObject(Result.Fail(localizer["You are not authorized to access this resource."]));
    //                    return context.Response.WriteAsync(result);
    //                },
    //            };
    //        });
    //    services.AddAuthorization(options =>
    //    {
    //        // Here I stored necessary permissions/roles in a constant
    //        foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
    //        {
    //            var propertyValue = prop.GetValue(null);
    //            if (propertyValue is not null)
    //            {
    //                options.AddPolicy(propertyValue.ToString(), policy => policy.RequireClaim(ApplicationClaimTypes.Permission, propertyValue.ToString()));
    //            }
    //        }
    //    });
    //    return services;
    //}

    public static IServiceCollection AddRazorViewRender(this IServiceCollection service)
    {
        service.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        service.AddScoped<IRenderViewService, RenderViewService>();
        return service;
    }

    public static IServiceCollection AddPdfGenerator(this IServiceCollection service)
    {
        service.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        service.AddScoped<IPdfGenerator, PdfGenerator>();
        return service;
    }

    public static IServiceCollection AddQrCodeGenerator(this  IServiceCollection service)
    {
        service.AddScoped<IGenerateQrCode, GenerateQrCode>();
        return service;
    }

    public static IServiceCollection AddRecurringJobServices(this  IServiceCollection service)
    {
        service.AddScoped<IGoToNextPeriodOfFestival, GoToNextPeriodOfFestival>();
        return service;
    }
}
