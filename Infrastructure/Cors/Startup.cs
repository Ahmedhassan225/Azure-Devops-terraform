using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Infrastructure.Cors
{
    internal static class Startup
    {
        private const string CorsPolicy = nameof(CorsPolicy);

        internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
        {
            var angularOrigin = config.GetSection("CorsSettings:Angular").Value;
            var origins = new List<string>();
            if (angularOrigin is not null)
                origins.AddRange(angularOrigin.Split(';', StringSplitOptions.RemoveEmptyEntries));

            return services.AddCors(opt =>
                opt.AddPolicy(CorsPolicy, policy =>
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithOrigins(origins.ToArray())));
        }

        internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) =>
            app.UseCors(CorsPolicy);
    }
}

