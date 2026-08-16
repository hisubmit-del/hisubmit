namespace HiSubmit.Client.Infrastructure.Routes
{
    public static class TokenEndpoints
    {
        public static string Get = "api/identity/token";
        public static string Verify = "api/identity/token/verify-email";
        public static string Resend = "api/identity/token/resend-email";
        public static string Refresh = "api/identity/token/refresh";
    }
}

