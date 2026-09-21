using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using UfBusiness = Sada.Api.Business.Uf;

namespace Sada.Api.Business.Test;

public sealed class UfTests
{
    [Fact]
    public void Uf_DeveImplementarInterfaceIUf()
    {
        var (service, _, _) = CriarServico();

        Assert.IsAssignableFrom<IUf>(service);
    }

    [Fact]
    public async Task ListarUfsAsync_QuandoExistemRegistros_DeveMapearTodosOsCampos()
    {
        var ufs = new List<UfModel>
        {
            new() { IdUf = 33, SiglaUf = "RJ", Sigla = "Rio de Janeiro" },
            new() { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" }
        };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarUfsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(ufs);

        var result = await service.ListarUfsAsync();

        Assert.Collection(result,
            x => AssertUf(x.IdUf, x.SiglaUf, x.Sigla, 33, "RJ", "Rio de Janeiro"),
            x => AssertUf(x.IdUf, x.SiglaUf, x.Sigla, 35, "SP", "Sao Paulo"));
        repository.Verify(x => x.ListarUfsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarUfsAsync_QuandoNaoExistemRegistros_DeveRetornarListaVazia()
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarUfsAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await service.ListarUfsAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListarUfsAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarUfsAsync(It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ListarUfsAsync());

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao listar UFs", expected);
    }

    [Fact]
    public async Task ObterUfPorIdAsync_QuandoExiste_DeveRetornarListaComUmItemMapeado()
    {
        const int idUf = 35;
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ObterUfPorIdAsync(idUf, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UfModel { IdUf = idUf, SiglaUf = "SP", Sigla = "Sao Paulo" });

        var result = await service.ObterUfPorIdAsync(idUf);

        var item = Assert.Single(result);
        AssertUf(item.IdUf, item.SiglaUf, item.Sigla, idUf, "SP", "Sao Paulo");
        repository.Verify(x => x.ObterUfPorIdAsync(idUf, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterUfPorIdAsync_QuandoNaoExiste_DeveLogarELancarInvalidOperationException()
    {
        const int idUf = 999;
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterUfPorIdAsync(idUf, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UfModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterUfPorIdAsync(idUf));

        Assert.Contains("UF", exception.Message);
        Assert.Contains("encontrada", exception.Message);
        VerificarLogErro(logger, "Erro ao listar UFs", exception);
    }

    [Fact]
    public async Task ObterUfPorIdAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterUfPorIdAsync(35, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterUfPorIdAsync(35));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao listar UFs", expected);
    }

    [Fact]
    public async Task IncluirUfAsync_DeveMapearRequestRepassarEntidadeERetornarResposta()
    {
        var request = new UfModelRequest { IdUf = 99, SiglaUf = " sp ", Sigla = " Sao Paulo " };
        var persisted = new UfModel { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" };
        var (service, repository, _) = CriarServico();
        UfModel? recebido = null;
        repository.Setup(x => x.IncluirUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()))
            .Callback<UfModel, CancellationToken>((model, _) => recebido = model)
            .ReturnsAsync(persisted);

        var result = await service.IncluirUfAsync(request);

        Assert.NotNull(recebido);
        AssertUf(recebido.IdUf, recebido.SiglaUf, recebido.Sigla, 99, " sp ", " Sao Paulo ");
        AssertUf(result.IdUf, result.SiglaUf, result.Sigla, 35, "SP", "Sao Paulo");
        repository.Verify(x => x.IncluirUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IncluirUfAsync_ComCamposNulos_DeveRepassarCamposNulos()
    {
        var request = new UfModelRequest { IdUf = 1, SiglaUf = null, Sigla = null };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.IncluirUfAsync(
                It.Is<UfModel>(m => m.IdUf == 1 && m.SiglaUf == null && m.Sigla == null),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UfModel { IdUf = 1 });

        var result = await service.IncluirUfAsync(request);

        Assert.Equal(1, result.IdUf);
        Assert.Null(result.SiglaUf);
        Assert.Null(result.Sigla);
    }

    [Fact]
    public async Task IncluirUfAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao incluir");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.IncluirUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.IncluirUfAsync(new UfModelRequest()));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao incluir UF", expected);
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoExiste_DeveMapearRequestEResposta()
    {
        var request = new UfModelRequest { IdUf = 35, SiglaUf = "RJ", Sigla = "Rio de Janeiro" };
        var persisted = new UfModel { IdUf = 35, SiglaUf = "RJ", Sigla = "Rio de Janeiro" };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.AlterarUfAsync(
                It.Is<UfModel>(m => m.IdUf == 35 && m.SiglaUf == "RJ" && m.Sigla == "Rio de Janeiro"),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(persisted);

        var result = await service.AlterarUfAsync(request);

        AssertUf(result.IdUf, result.SiglaUf, result.Sigla, 35, "RJ", "Rio de Janeiro");
        repository.Verify(x => x.AlterarUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoNaoExiste_DeveLogarELancarKeyNotFoundException()
    {
        var request = new UfModelRequest { IdUf = 999, SiglaUf = "XX", Sigla = "Inexistente" };
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UfModel?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AlterarUfAsync(request));

        Assert.Equal("UF com ID 999 não encontrada.", exception.Message);
        VerificarLogErro(logger, "Erro ao alterar UF", exception);
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao alterar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarUfAsync(It.IsAny<UfModel>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.AlterarUfAsync(new UfModelRequest { IdUf = 35 }));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao alterar UF", expected);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ApagarUfAsync_DeveRetornarResultadoDoRepositorio(bool resultadoRepositorio)
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ApagarUfAsync(35, It.IsAny<CancellationToken>()))
            .ReturnsAsync(resultadoRepositorio);

        var result = await service.ApagarUfAsync(35);

        Assert.Equal(resultadoRepositorio, result);
        repository.Verify(x => x.ApagarUfAsync(35, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApagarUfAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarUfAsync(35, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApagarUfAsync(35));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao apagar UF", expected);
    }

    private static (
        UfBusiness Service,
        Mock<IUfRepository> Repository,
        Mock<ILogger<UfBusiness>> Logger) CriarServico()
    {
        var repository = new Mock<IUfRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UfBusiness>>();
        return (new UfBusiness(logger.Object, repository.Object), repository, logger);
    }

    private static void AssertUf(
        int id, string? siglaUf, string? sigla, int idEsperado, string? siglaUfEsperada, string? siglaEsperada)
    {
        Assert.Equal(idEsperado, id);
        Assert.Equal(siglaUfEsperada, siglaUf);
        Assert.Equal(siglaEsperada, sigla);
    }

    private static void VerificarLogErro(
        Mock<ILogger<UfBusiness>> logger, string mensagem, Exception exception)
    {
        logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) => state.ToString() == mensagem),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
