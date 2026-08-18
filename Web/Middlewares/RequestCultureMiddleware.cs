using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Middlewares
{
    public class RequestCultureMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestCultureMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var cultureQuery = context.Request.Query["culture"];
            if (TryGetCulture(cultureQuery, out var culture))
            {
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;
            }
            else if (context.Request.Headers.ContainsKey("Accept-Language"))
            {
                var cultureHeader = context.Request.Headers["Accept-Language"];
                if (cultureHeader.Any())
                {
                    var requestedCulture = cultureHeader.First()
                        .Split(',')
                        .Select(value => value.Split(';').First().Trim())
                        .FirstOrDefault(value => TryGetCulture(value, out _));

                    if (TryGetCulture(requestedCulture, out culture))
                    {
                        CultureInfo.CurrentCulture = culture;
                        CultureInfo.CurrentUICulture = culture;
                    }
                }
            }

            await _next(context);
        }

        private static bool TryGetCulture(string? value, out CultureInfo culture)
        {
            culture = null!;
            if (string.IsNullOrWhiteSpace(value) || value == "*")
                return false;

            try
            {
                culture = new CultureInfo(value);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
