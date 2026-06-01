using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Repository;

namespace Sada.Api.Entity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSadaEntityFramework(this IServiceCollection services)
    {
        services.AddDbContext<SadaDbContext>(options =>
            options.UseInMemoryDatabase("SadaDb"));
        services.AddScoped<ITituloRepository, TituloRepository>();

        return services;
    }
}
