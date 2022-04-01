using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Common.Utils;

public static class HostEnvironmentExtensions
{
    public static Logger BuildLogger(this IHostEnvironment hostEnvironment) =>
        new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Filter.ByExcluding("RequestPath = '/health' and StatusCode = 200")
            .WriteTo.Logger(lc =>
            {
                if (hostEnvironment.IsDevelopment())
                {
                    lc.WriteTo.Console();
                }
                else
                {
                    lc.WriteTo.Console(new RenderedCompactJsonFormatter());
                }
            })
            .CreateLogger();
}
