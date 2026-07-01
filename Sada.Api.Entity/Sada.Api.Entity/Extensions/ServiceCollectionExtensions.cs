using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Repository;

namespace Sada.Api.Entity.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSadaEntityFramework(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("SadaDatabase")
            ?? throw new InvalidOperationException(
                "A connection string 'ConnectionStrings:SadaDatabase' precisa estar configurada.");

        services.AddDbContext<SadaDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<ITituloRepository, TituloRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }
}
