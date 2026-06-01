using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sada.Api.Business;
using Sada.Api.Business.Interface;
using Sada.Application.Configuration;

namespace Sada.Application.Test;

public sealed class DependencyInjectionConfigurationsTests
{
    [Fact]
    public void AddDependencyInjectionConfiguration_ComServicesNulo_DeveLancarArgumentNullException()
    {
        IServiceCollection? services = null;
        var configuration = CriarConfiguration();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            services!.AddDependencyInjectionConfiguration(configuration));

        Assert.Equal("services", exception.ParamName);
    }

    [Fact]
    public void AddDependencyInjectionConfiguration_ComConfigurationNula_DeveLancarArgumentNullException()
    {
        var services = new ServiceCollection();

        var exception = Assert.Throws<ArgumentNullException>(() =>
            services.AddDependencyInjectionConfiguration(null!));

        Assert.Equal("configuration", exception.ParamName);
    }

    [Fact]
    public void AddDependencyInjectionConfiguration_DeveRetornarMesmaColecaoERegistrarServicos()
    {
        var services = new ServiceCollection();
        var configuration = CriarConfiguration();

        var returnedServices = services.AddDependencyInjectionConfiguration(configuration);

        Assert.Same(services, returnedServices);
        Assert.Contains(services, item =>
            item.ServiceType.FullName == "Sada.Application.Middlewares.ExceptionHandlingMiddleware" &&
            item.Lifetime == ServiceLifetime.Transient);
        Assert.Contains(services, item =>
            item.ServiceType == typeof(ITitulo) &&
            item.ImplementationType == typeof(Titulo) &&
            item.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services, item =>
            item.ServiceType.FullName == "Microsoft.Extensions.Caching.Memory.IMemoryCache");
        Assert.Contains(services, item =>
            item.ServiceType.FullName == "Microsoft.AspNetCore.Mvc.Infrastructure.IActionInvokerFactory");
    }

    private static IConfiguration CriarConfiguration()
    {
        return new ConfigurationBuilder().Build();
    }
}
