using Infrastructure.Context.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    internal static class Startup
    {

        internal static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration config)
        {    
            services
                .AddDbContext<DatabaseContext>(options => 
                    options.UseSqlServer(config.GetConnectionString("DB") ?? string.Empty))
                .AddScoped<DbContext, DatabaseContext>();

            return services;
        }
    }
}