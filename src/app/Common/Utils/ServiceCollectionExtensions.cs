using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Utils
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTransientByConvention(
            this IServiceCollection services,
            Type assemblyMarkerType,
            Func<Type, bool> predicate)
            => services.AddTransientByConvention(new List<Type> { assemblyMarkerType }, predicate);

        public static IServiceCollection AddTransientByConvention(
            this IServiceCollection services,
            IEnumerable<Type> assemblyMarkerTypes,
            Func<Type, bool> predicate)
            => services.AddTransientByConvention(assemblyMarkerTypes, predicate, predicate);

        private static IServiceCollection AddTransientByConvention(
            this IServiceCollection services,
            IEnumerable<Type> assemblyMarkerTypes,
            Func<Type, bool> interfacePredicate,
            Func<Type, bool> implementationPredicate)
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
    }
}
