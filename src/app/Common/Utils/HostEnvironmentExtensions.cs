using Microsoft.Extensions.Hosting;

namespace Common.Utils
{
    public static class HostEnvironmentExtensions
    {
        public static bool IsDevelopmentDocker(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsEnvironment("DevelopmentDocker");
        }

        public static bool IsLocal(this IHostEnvironment hostEnvironment)
        {
            return hostEnvironment.IsDevelopment() || hostEnvironment.IsDevelopmentDocker();
        }
    }
}
