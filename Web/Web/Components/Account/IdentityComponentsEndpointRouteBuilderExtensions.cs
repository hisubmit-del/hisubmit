using HiSubmit.Infrastructure.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Text.Json;
using HiSubmit.Client.SharedModels.Constants.Role;
using Hisubmit.Client.SharedModels.Contracts.Permission;
using Microsoft.AspNetCore.Authorization;
using Web.Components.Account.Pages;
using Web.Components.Account.Pages.Manage;


namespace Microsoft.AspNetCore.Routing
{
    internal static class IdentityComponentsEndpointRouteBuilderExtensions
    {
        // These endpoints are required by the Identity Razor components defined in the /Components/Account/Pages directory of this project.
        public static IEndpointConventionBuilder MapAdditionalIdentityEndpoints(this IEndpointRouteBuilder endpoints)
        {
            ArgumentNullException.ThrowIfNull(endpoints);

            var accountGroup = endpoints.MapGroup("/Account");

            accountGroup.MapPost("/PerformExternalLogin", (
                HttpContext context,
                [FromServices] SignInManager<BlazorHeroUser> signInManager,
                [FromForm] string provider,
                [FromForm] string returnUrl) =>
            {
                IEnumerable<KeyValuePair<string, StringValues>> query = [
                    new("ReturnUrl", returnUrl),
                    new("Action", ExternalLogin.LoginCallbackAction)];

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/ExternalLogin",
                    QueryString.Create(query));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return TypedResults.Challenge(properties, [provider]);
            });

            accountGroup.MapPost("/Logout", async (
                ClaimsPrincipal user,
                HttpContext context,
                SignInManager<BlazorHeroUser> signInManager,
                [FromForm] string returnUrl) =>
            {
                await signInManager.SignOutAsync();
                context.Response.Cookies.Delete(ApplicationClaimTypes.AdminLoginFestival);
                context.Response.Cookies.Delete(ApplicationClaimTypes.SelectedFestival);
                return TypedResults.LocalRedirect($"~/{returnUrl}");
            }).DisableAntiforgery();

            var manageGroup = accountGroup.MapGroup("/Manage").RequireAuthorization();

            manageGroup.MapPost("/LinkExternalLogin", async (
                HttpContext context,
                [FromServices] SignInManager<BlazorHeroUser> signInManager,
                [FromForm] string provider) =>
            {
                // Clear the existing external cookie to ensure a clean login process
                await context.SignOutAsync(IdentityConstants.ExternalScheme);

                var redirectUrl = UriHelper.BuildRelative(
                    context.Request.PathBase,
                    "/Account/Manage/ExternalLogins",
                    QueryString.Create("Action", ExternalLogins.LinkLoginCallbackAction));

                var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, signInManager.UserManager.GetUserId(context.User));
                return TypedResults.Challenge(properties, [provider]);
            });

            var loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var downloadLogger = loggerFactory.CreateLogger("DownloadPersonalData");

            manageGroup.MapPost("/DownloadPersonalData", async (
                HttpContext context,
                [FromServices] UserManager<BlazorHeroUser> userManager,
                [FromServices] AuthenticationStateProvider authenticationStateProvider) =>
            {
                var user = await userManager.GetUserAsync(context.User);
                if (user is null)
                {
                    return Results.NotFound($"Unable to load user with ID '{userManager.GetUserId(context.User)}'.");
                }

                var userId = await userManager.GetUserIdAsync(user);
                downloadLogger.LogInformation("User with ID '{UserId}' asked for their personal data.", userId);

                // Only include personal data for download
                var personalData = new Dictionary<string, string>();
                var personalDataProps = typeof(BlazorHeroUser).GetProperties().Where(
                    prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
                foreach (var p in personalDataProps)
                {
                    personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
                }

                var logins = await userManager.GetLoginsAsync(user);
                foreach (var l in logins)
                {
                    personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
                }

                personalData.Add("Authenticator Key", (await userManager.GetAuthenticatorKeyAsync(user))!);
                var fileBytes = JsonSerializer.SerializeToUtf8Bytes(personalData);

                context.Response.Headers.TryAdd("Content-Disposition", "attachment; filename=PersonalData.json");
                return TypedResults.File(fileBytes, contentType: "application/json", fileDownloadName: "PersonalData.json");
            });


            manageGroup.MapPost("/select-account", async (
                HttpContext context,
               [FromForm] int? FestivalId,
                [FromForm] string returnUrl) =>
            {
                var cookieName = ApplicationClaimTypes.SelectedFestival;

                if (FestivalId != null)
                {
                    context.Response.Cookies.Append(
                        cookieName,
                        FestivalId.ToString()!,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(3),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = false
                        });
                }
                else
                {
                    context.Response.Cookies.Delete(cookieName);
                }

                return TypedResults.LocalRedirect($"~/{returnUrl}");
            }).DisableAntiforgery();

            manageGroup.MapPost("/admin-login-to-festival",
                [Authorize(Roles = RoleConstants.AdministratorRole)] async (
                HttpContext context,
                [FromForm] int? FestivalId,
                [FromForm] string returnUrl) =>
            {
                var cookieName = ApplicationClaimTypes.AdminLoginFestival;

                if (FestivalId != null)
                {
                    context.Response.Cookies.Append(
                        cookieName,
                        FestivalId.ToString()!,
                        new CookieOptions
                        {
                            Expires = DateTimeOffset.UtcNow.AddDays(1),
                            HttpOnly = true,
                            Secure = true,
                            IsEssential = false
                        });
                }
                else
                {
                    context.Response.Cookies.Delete(cookieName);
                }

                return TypedResults.LocalRedirect($"~/{returnUrl}");
            }).DisableAntiforgery();


            manageGroup.MapPost("/admin-logout-from-festival",
                [Authorize(Roles = RoleConstants.AdministratorRole)] async (
                    HttpContext context,
                    [FromForm] int? FestivalId,
                    [FromForm] string returnUrl) =>
                {
                    var cookieName = ApplicationClaimTypes.AdminLoginFestival;

                    context.Response.Cookies.Delete(cookieName);

                    return TypedResults.LocalRedirect($"~/{returnUrl}");
                }).DisableAntiforgery();
            return accountGroup;
        }
    }
}
