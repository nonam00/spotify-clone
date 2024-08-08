using Microsoft.AspNetCore.Antiforgery;

namespace WebAPI.Middleware
{
    public class XsrfProtectionMiddleware(RequestDelegate next, IAntiforgery antiforgery)
    {
        private readonly IAntiforgery _antiforgery = antiforgery;
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Headers.ContainsKey("X-Xsrf-Token") && 
                context.Request.Headers["X-Xsrf-Token"] == "")
            {
                var token = _antiforgery.GetTokens(context).RequestToken!;
                Console.WriteLine(token);
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
