using Common.Dal;
using Common.Dal.Interfaces;
using Common.Services.NoSql.Domain.Interfaces;
using Common.Utils;
using IIdGenerator = Common.Dal.Interfaces.IIdGenerator;

namespace Api.NoSql
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDi(this IServiceCollection services)
        {
            // replace with simpler version, if SQL DAL is removed from the solution:
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
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository"),
                t => t.Namespace.StartsWith("Common.Dal.") && t.Name.EndsWith("Repository")
            );

            Predicate<Type> predicate = t =>
                (
                    t.Namespace.StartsWith("Common.Services.NoSql.") ||
                    t.Namespace.StartsWith("Common.Services.Infrastructure.")
                )
                && t.Name.EndsWith("Service");

            services.AddTransientByConvention(
                new List<Type> { typeof(IUserService) },
                predicate,
                predicate
            );

            services.AddTransient<IDbContext, DbContext>();

            services.AddTransient<IIdGenerator, IdGenerator>();
        }
    }
}
