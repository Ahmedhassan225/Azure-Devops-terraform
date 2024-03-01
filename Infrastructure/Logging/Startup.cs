
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Infrastructure.Logging
{
    public static class Startup
    {
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration config)
        {
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .CreateLogger();

            return services.AddSerilog(logger);
        }
    }
}

