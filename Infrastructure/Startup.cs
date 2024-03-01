using Infrastructure.Cors;
using Infrastructure.Logging;
using Infrastructure.Mapping;
using Infrastructure.Middleware;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            MapsterSettings.Configure();
            services.AddMapster();

            return services.AddCorsPolicy(config)
                            .AddSerilogLogging(config)
                            .AddExceptionMiddleware()
                            .AddPersistence(config);
        }

        public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder) =>
            builder
                .UseCorsPolicy()
                .UseExceptionMiddleware();

    }
}

