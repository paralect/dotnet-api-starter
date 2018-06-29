using Api.Core.Abstract;
using Api.Dal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests.Setup
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Api.Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureTestServices(services =>
            {
                services.AddTransient<IMongoDbContext, TestMongoDbContext>();
            });
        }

        public void DropDatabase()
        {
            IWebHost host = Server?.Host;
            if (host == null)
                throw new Exception("CreateClient method should be called before calling DropDatabase");

            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                var dbContext = (TestMongoDbContext)services.GetRequiredService<IMongoDbContext>();
                dbContext.DropDatabase();
            }
        }

        public T GetService<T>()
        {
            IWebHost host = Server?.Host;
            if (host == null)
                throw new Exception("CreateClient method should be called before calling DropDatabase");

            using (var scope = host.Services.CreateScope())
            {
                IServiceProvider services = scope.ServiceProvider;
                return services.GetRequiredService<T>();
            }
        }
    }
}
