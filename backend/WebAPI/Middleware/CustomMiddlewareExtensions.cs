using Microsoft.AspNetCore.Antiforgery;

namespace WebAPI.Middleware
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(
            this IApplicationBuilder builder) => 
            builder.UseMiddleware<CustomExceptionHandlerMiddleware>();

        public static IApplicationBuilder UseXsrfProtection(
            this IApplicationBuilder builder, IAntiforgery antiforgery)
            => builder.UseMiddleware<XsrfProtectionMiddleware>(antiforgery);

        public static IApplicationBuilder UseAuthenticationMiddleware(
            this IApplicationBuilder builder) =>
            builder.UseMiddleware<AuthenticationMiddleware>();
    }
}
