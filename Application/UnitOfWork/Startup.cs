using Application.UnitOfWork;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Services
{
    internal static class Startup
    {
        internal static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            return services
                .AddScoped<IIdentityServiceUOW, IdentityServiceUOW>();
        }
    }
}