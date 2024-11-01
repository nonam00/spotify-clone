using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Primitives;

namespace WebAPI.Middleware
{
    public class XsrfProtectionMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        private readonly IAntiforgery _antiforgery = antiforgery;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("X-Xsrf-Token", out StringValues value) &&
value == "")
            {
                var token = _antiforgery.GetTokens(context).RequestToken!;
                context.Response.Cookies.Append(".AspNetCore.Xsrf", token, new CookieOptions
                {
                    HttpOnly = false,
                    SameSite = SameSiteMode.None, 
                    MaxAge = TimeSpan.FromMinutes(60)
                });
            }

            await _next(context);
        }
    }
}
