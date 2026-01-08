using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

using Infrastructure.Auth;
using WebAPI.Services;

namespace WebAPI;

public static class DependencyInjection
{
    // Setting CORS policy for local responds
    extension(IServiceCollection services)
    {
        public IServiceCollection AddLocalCorsPolicy()
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .SetIsOriginAllowed(_ => true)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
        
                    policy.WithOrigins("http://localhost:5173")
                        .SetIsOriginAllowed(_ => true)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });
        
            return services;
        }

        public IServiceCollection AddAuthServices(IConfiguration configuration)
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
                            Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        RoleClaimType = ClaimTypes.Role
                    };
        
                    // Getting JWT token from request cookies
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Request.Cookies.TryGetValue("access_token", out var token);
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

            services.AddAuthorizationBuilder()
                .AddPolicy(AuthorizationPolicies.UserOnly, policy =>
                    policy.RequireClaim(CustomClaims.EntityType, EntityTypes.User))
                .AddPolicy(AuthorizationPolicies.ModeratorOnly, policy =>
                    policy.RequireClaim(CustomClaims.EntityType, EntityTypes.Moderator))
                .AddPolicy(AuthorizationPolicies.CanManageUsers, policy =>
                    policy.RequireClaim(CustomClaims.Permission, Permissions.ManageUsers))
                .AddPolicy(AuthorizationPolicies.CanManageContent, policy =>
                    policy.RequireClaim(CustomClaims.Permission, Permissions.ManageContent))
                .AddPolicy(AuthorizationPolicies.CanManageModerators, policy =>
                    policy.RequireClaim(CustomClaims.Permission, Permissions.ManageModerators))
                .AddPolicy(AuthorizationPolicies.CanViewReports, policy =>
                    policy.RequireClaim(CustomClaims.Permission, Permissions.ViewReports));
        
            return services;
        }

        public IServiceCollection AddBackgroundServices()
        {
            services.AddHostedService<RefreshTokensCleanupService>();
            services.AddHostedService<NonActiveUsersCleanupService>();
            return services;
        }
    }
    
    // Authentication final config in DI
}

public static class AuthorizationPolicies
{
    public const string UserOnly = "UserOnly";
    public const string ModeratorOnly = "ModeratorOnly";
    public const string CanManageUsers = "CanManageUsers";
    public const string CanManageContent = "CanManageContent";
    public const string CanManageModerators = "CanManageModerators";
    public const string CanViewReports = "CanViewReports";
}