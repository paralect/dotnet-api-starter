using Common.DalSql.Interfaces;
using Common.Services.Sql.Domain.Interfaces;
using Common.Utils;

namespace Api.Sql
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDi(this IServiceCollection services)
        {
            // replace with simpler version, if MongoDB DAL is removed from the solution:
            // services.AddTransientByConvention(
            //     typeof(IRepository<,>),
            //     t => t.Name.EndsWith("Repository")
            // );

            // services.AddTransientByConvention(
            //     new List<Type> { typeof(IAuthService), typeof(IUserService) },
            //     t => t.Name.EndsWith("Service")
            // );

            services.AddTransientByConvention(
                new List<Type> { typeof(IRepository<,>) },
                t => t.Namespace.StartsWith("Common.DalSql.") && t.Name.EndsWith("Repository"),
                t => t.Namespace.StartsWith("Common.DalSql.") && t.Name.EndsWith("Repository")
            );

            Predicate<Type> predicate = t =>
                (
                    t.Namespace.StartsWith("Common.Services.Sql.") ||
                    t.Namespace.StartsWith("Common.Services.Infrastructure.")
                )
                && t.Name.EndsWith("Service");

            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                predicate,
                predicate
            );
        }
    }
}
