using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
using Prometheus.SystemMetrics;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

using Application;
using Infrastructure;
using Persistence;
using WebAPI;
using WebAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddLogging();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddLocalCorsPolicy();

// Adding and configuration authentication by JWT Tokens
builder.Services.AddAuthServices(builder.Configuration);

builder.Services.AddBackgroundServices();

builder.Services.AddControllersWithViews();

// Adding and Configuration API Versioning
builder.Services.AddApiVersioning()
                .AddApiExplorer(options => options.GroupNameFormat = "'v'VVV");

builder.Services.AddHealthChecks()
                .ForwardToPrometheus();

// Adding Prometheus metrics
builder.Services.AddMetrics()
                .AddSystemMetrics();

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

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        var notAppliedMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (notAppliedMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
    catch
    {
        await dbContext.Database.MigrateAsync();
    }
}

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

// Prometheus metrics middleware
app.UseHttpMetrics(options =>
{
    options.ReduceStatusCodeCardinality();
});
app.UseMetricServer();

app.UseCors("MyPolicy");

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapMetrics();
app.MapControllers();

app.Run();
