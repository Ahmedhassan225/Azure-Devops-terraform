
using Application.Repository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Repository
{
    internal static class Startup
    {

        internal static IServiceCollection AddRepository(this IServiceCollection services)
        {    
            return services
                .AddTransient<IRepositoryProvider, RepositoryProvider>()
                .AddTransient<IRepositoryFactory, RepositoryFactory>();
        }
    }
}