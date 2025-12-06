using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebAPI;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options) 
    {
        foreach(var description in _provider.ApiVersionDescriptions)
        {
            var apiVersion = description.ApiVersion.ToString();
            options.SwaggerDoc(description.GroupName,
                new OpenApiInfo
                {
                    Version = apiVersion,
                    Title = $"Spotify Clone API {apiVersion}",
                    Description = "API for Spotify Clone Web Application"
                });

            options.AddSecurityDefinition($"AuthToken {apiVersion}", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "bearer",
                Name = "Authorization",
                Description = "Authorization token",
            });
            
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            { 
                [new OpenApiSecuritySchemeReference("bearer", document)] = []
            });

            options.CustomOperationIds(apiDescription =>
                apiDescription.TryGetMethodInfo(out var methodInfo)
                    ? methodInfo.Name
                    : null);
        }
    }
}