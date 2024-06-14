using Microsoft.AspNetCore.Antiforgery;

namespace WebAPI.Middleware
{
    public class XsrfProtectionMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        private readonly IAntiforgery _antiforgery = antiforgery;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.Cookies.Append(
                ".AspNetCore.Xsrf",
                _antiforgery.GetAndStoreTokens(context).RequestToken!,
                new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    MaxAge = TimeSpan.FromMinutes(60)
                });

            await _next(context);
        }
    }
}
