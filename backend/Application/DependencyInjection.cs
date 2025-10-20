using FluentValidation;

using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using Application.Shared.Messaging;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all handlers from assembly
        var assembly = Assembly.GetExecutingAssembly();
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.GetInterfaces().Any(i => 
                i.IsGenericType && 
                (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                 i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                 i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            // Register handler as its implemented interface
            var implementedInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) ||
                     i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .ToList();

            foreach (var interfaceType in implementedInterfaces)
            {
                services.AddScoped(interfaceType, handlerType);
            }
        }
        
        // Register validators
        services.AddValidatorsFromAssemblies([assembly]);
        
        // Register custom mediator
        services.AddScoped<IMediator, Mediator>();
        
        return services;
    }
}