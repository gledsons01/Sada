using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Extensions;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddSadaEntityFramework_DeveRegistrarContextoERepositorio()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguration();

        var returnedServices = services.AddSadaEntityFramework(configuration);

        Assert.Same(services, returnedServices);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<SadaDbContext>();
        var repository = scope.ServiceProvider.GetRequiredService<ITituloRepository>();

        Assert.NotNull(context);
        Assert.IsType<TituloRepository>(repository);
        Assert.Contains("SqlServer", context.Database.ProviderName);
    }

    [Fact]
    public void AddSadaEntityFramework_DeveRegistrarRepositorioComTempoDeVidaScoped()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguration();

        services.AddSadaEntityFramework(configuration);

        var descriptor = Assert.Single(services, item => item.ServiceType == typeof(ITituloRepository));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        Assert.Equal(typeof(TituloRepository), descriptor.ImplementationType);
    }

    private static IConfiguration CriarConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:SadaDatabase"] =
                    "Server=(localdb)\\MSSQLLocalDB;Database=SADA_BD_TEST;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
            })
            .Build();
    }
}
