using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

using Application;
using Infrastructure;
using Persistence;

using WebAPI;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// Setting CORS policy for local responds
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .SetIsOriginAllowed(_ => true)
              .SetIsOriginAllowedToAllowWildcardSubdomains()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Adding and configuration authentication by JWT Tokens
builder.Services.AddAuthServices(builder.Configuration);

// Not only controllers because of XSRF protection working principle
builder.Services.AddControllersWithViews();

// Adding and Configuration API Versioning
builder.Services.AddApiVersioning()
                .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

app.UseExceptionHandler();

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

app.UseRouting();
app.UseCors("MyPolicy");

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
