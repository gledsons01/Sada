using System;
using Sada.Api.Business;
using Sada.Api.Business.Interface;
using Sada.Application.Middlewares;
using Sada.Application.Services;

namespace Sada.Application.Configuration
{
    public static class DependencyInjectionConfigurations
    {
        public static IServiceCollection AddDependencyInjectionConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            services.AddTransient<ExceptionHandlingMiddleware>();
            services.AddMemoryCache();

            services.AddMvc();
            services.AddScoped<ITitulo, Titulo>();
            services.AddScoped<IUsuario, Usuario>();
            services.AddScoped<IJwtTokenService, JwtTokenService>();

            return services;
        }
    }
}
