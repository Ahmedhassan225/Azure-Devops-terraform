using Domain.Shared;
using Infrastructure.Middleware.Exceptions;
using Mapster;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure.Mapping
{
    internal static class Startup
    {
        public static void AddMapster(this IServiceCollection services)
        {
            var typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
            typeAdapterConfig.Scan(typeof(BaseDTO<,>).Assembly);
        }

    }
}