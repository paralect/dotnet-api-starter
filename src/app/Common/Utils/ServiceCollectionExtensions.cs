using System.Reflection;
using Common.Caching;
using Common.Caching.Interfaces;
using Common.Security;
using Common.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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

    public static TSettings ConfigureSettings<TSettings>(
        this IServiceCollection services,
        IConfiguration configuration,
        string sectionName)
        where TSettings : class, new()
    {
        var setting = new TSettings();
        configuration.GetSection(sectionName).Bind(setting);

        services.AddSingleton(Options.Create(setting));

        return setting;
    }

    public static void ConfigureCors(this IServiceCollection services, AppSettings appSettings)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.CorsPolicy.AllowSpecificOrigin, builder =>
            {
                builder
                    .WithOrigins(appSettings.LandingUrl, appSettings.WebUrl)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
    }

    public static void ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(o => o.Filters.Add(typeof(ValidationAttribute)))
            .ConfigureApiBehaviorOptions(o =>
            {
                o.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState.GetErrors();
                    var result = new BadRequestObjectResult(errors);

                    return result;
                };
            });
    }

    public static void ConfigureCache(this IServiceCollection services, CacheSettings cacheSettings)
    {
        services.AddStackExchangeRedisCache(options => options.Configuration = cacheSettings.ConnectionString);

        services.AddTransient<ICache, Cache>();

        if (cacheSettings.IsEnabled)
        {
            services
                .AddHealthChecks()
                .AddRedis(cacheSettings.ConnectionString);
        }
    }

    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
        });
    }
}