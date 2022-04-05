using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Common.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Utils;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTransientByConvention(
        this IServiceCollection services,
        Type assemblyMarkerType,
        Predicate<Type> predicate)
        => services.AddTransientByConvention(new List<Type> { assemblyMarkerType }, predicate);

    public static IServiceCollection AddTransientByConvention(
        this IServiceCollection services,
        IEnumerable<Type> assemblyMarkerTypes,
        Predicate<Type> predicate)
        => services.AddTransientByConvention(assemblyMarkerTypes, predicate, predicate);

    public static IServiceCollection AddTransientByConvention(
        this IServiceCollection services,
        IEnumerable<Type> assemblyMarkerTypes,
        Predicate<Type> interfacePredicate,
        Predicate<Type> implementationPredicate)
    {
        var assemblies = assemblyMarkerTypes.Select(Assembly.GetAssembly).ToList();

        var interfaces = assemblies.SelectMany(a => a.ExportedTypes)
            .Where(x => x.IsInterface && interfacePredicate(x))
            .ToList();

        var implementations = assemblies.SelectMany(a => a.ExportedTypes)
            .Where(x => !x.IsInterface && !x.IsAbstract && implementationPredicate(x))
            .ToList();

        foreach (var @interface in interfaces)
        {
            var interfaceImplementations = implementations
                .Where(x => @interface.IsAssignableFrom(x))
                .ToList();

            if (interfaceImplementations.Count == 1)
            {
                services.AddTransient(@interface, interfaceImplementations.First());
            }
        }

        return services;
    }

    public static void ConfigureHealthChecks(this IServiceCollection services, DbSettings dbSettings)
    {
        services
            .AddHealthChecks()
            .AddMongoDb(
                mongodbConnectionString: dbSettings.ConnectionString,
                mongoDatabaseName: dbSettings.Database
            );
    }
}
