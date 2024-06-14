using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;

using Application;
using Application.Interfaces;
using Application.Common.Mappings;

using Infrastructure;
using Persistence;

using WebAPI;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Adding and configurating AutoMapper
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
    config.AddProfile(new AssemblyMappingProfile(typeof(ISongsDbContext).Assembly));
});

// Adding application layer via dependency injection
builder.Services.AddApplication();

// Adding infrastructure layer via dependency injection
builder.Services.AddInfrastructure(builder.Configuration);

// Adding persistence layer via dependency injection
builder.Services.AddPersistence(builder.Configuration);

// Setting CORS policy for local responds
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:1",
                  "https://localhost:3000", "https://localhost:1")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Adding and configuration authentication by JWT Tokens

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // Getting options for JWT crypt from configuration (user secret)
        var jwtOptions = builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();

        options.TokenValidationParameters = new()
        {
            // TODO: replace with real issuer and real audience 
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
        };
        
        // Getting JWT token from request cookies
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["token"];
                return Task.CompletedTask;  
            }
        };

        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
    });

// Configurating XSRF protection
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "x-xsrf-token";
});

// Not only controllers because of XSRF protection working principle
builder.Services.AddControllersWithViews(); 

// Adding and Configuration API Versioning
builder.Services.AddApiVersioning()
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                });

// Adding Swagger for testing http requests
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
    builder.Services.AddSwaggerGen(config =>
    {
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        config.IncludeXmlComments(xmlPath);
    });
}

var app = builder.Build();

// Adding Swagger for testing http requests
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            config.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                description.GroupName.ToUpperInvariant());
            config.RoutePrefix = string.Empty;
        }
    });
}

app.UseCustomExceptionHandler();
app.UseRouting();
app.UseHttpsRedirection();
app.UseCors("MyPolicy");

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest
});

// Adding authentication middleware
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["token"];
    if (!string.IsNullOrEmpty(token))
        context.Request.Headers.Append("Authorization", "Bearer " + token);

    await next();
});

app.UseAuthentication();
app.UseAuthorization();

// Adding XSRF protection
var antiforgery = app.Services.GetRequiredService<IAntiforgery>();
app.UseXsrfProtection(antiforgery);

app.MapControllers();

app.Run();
