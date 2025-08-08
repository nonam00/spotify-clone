using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Infrastructure.Auth;

namespace WebAPI;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                // Getting options for JWT crypt from configuration
                var jwtOptions = configuration
                    .GetRequiredSection(nameof(JwtOptions))
                    .Get<JwtOptions>() ?? throw new NullReferenceException("JWT options not found");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // TODO: replace with real issuer and real audience 
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
        
                // Getting JWT token from request cookies
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Request.Cookies.TryGetValue("token", out var token);
                        if (!string.IsNullOrEmpty(token))
                        {
                            context.Token = token;
                        }
                        return Task.CompletedTask;  
                    }
                };

                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
            });

        return services;
    }
}