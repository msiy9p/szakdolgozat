using Libellus.Application.Behaviors;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Libellus.Application.Common.Interfaces.Security;

namespace Libellus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration config)
    {
        var assemblies = new[] { typeof(DependencyInjection).Assembly, Assembly.GetExecutingAssembly() };

        services.AddMediatR(assemblies);
        services.AddFluentValidation(assemblies);

        // ORDER MATTERS!
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Behaviors.ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthorisationBehavior<,>));

        AddAuthorisationHandlers(services, typeof(DependencyInjection).Assembly);
        AddAuthorisersFromAssembly(services, typeof(DependencyInjection).Assembly);

        return services;
    }

    private static IServiceCollection AddAuthorisationHandlers(IServiceCollection services, Assembly assembly)
    {
        var authHandlerOpenType = typeof(IAuthorisationHandler<>);
        GetTypesAssignableTo(assembly, authHandlerOpenType)
            .ForEach((concreation) =>
            {
                foreach (var implementedInterface in concreation.ImplementedInterfaces)
                {
                    if (!implementedInterface.IsGenericType)
                    {
                        continue;
                    }

                    if (implementedInterface.GetGenericTypeDefinition() != authHandlerOpenType)
                    {
                        continue;
                    }

                    services.AddTransient(implementedInterface, concreation);
                }
            });

        return services;
    }

    private static void AddAuthorisersFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        var authoriserType = typeof(IAuthorisationPolicyBuilder<>);
        GetTypesAssignableTo(assembly, authoriserType).ForEach((type) =>
        {
            foreach (var implementedInterface in type.ImplementedInterfaces)
            {
                if (!implementedInterface.IsGenericType)
                {
                    continue;
                }

                if (implementedInterface.GetGenericTypeDefinition() != authoriserType)
                {
                    continue;
                }

                switch (lifetime)
                {
                    case ServiceLifetime.Scoped:
                        services.AddScoped(implementedInterface, type);
                        break;
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(implementedInterface, type);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(implementedInterface, type);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
                }
            }
        });
    }

    private static List<TypeInfo> GetTypesAssignableTo(Assembly assembly, Type compareType)
    {
        var typeInfoList = assembly.DefinedTypes.Where(x => x.IsClass
                                                            && !x.IsAbstract
                                                            && x != compareType
                                                            && x.GetInterfaces()
                                                                .Any(i => i.IsGenericType
                                                                          && i.GetGenericTypeDefinition() ==
                                                                          compareType))?.ToList();

        return typeInfoList ?? new List<TypeInfo>();
    }
}