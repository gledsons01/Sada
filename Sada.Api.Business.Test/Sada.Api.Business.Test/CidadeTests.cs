using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using CidadeBusiness = Sada.Api.Business.Cidade;

namespace Sada.Api.Business.Test;

public sealed class CidadeTests
{
    [Fact]
    public void Cidade_DeveImplementarInterfaceICidade()
    {
        var (service, _, _) = CriarServico();

        Assert.IsAssignableFrom<ICidade>(service);
    }

    [Fact]
    public async Task ListarCidadesAsync_QuandoExistemRegistros_DeveMapearTodosOsCampos()
    {
        var cidades = new List<CidadeModel>
        {
            CriarModel(1, 35, "Sao Paulo"),
            CriarModel(2, 33, "Rio de Janeiro")
        };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarCidadesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(cidades);

        var result = await service.ListarCidadesAsync();

        Assert.Collection(result,
            x => AssertCidade(x.IdCidade, x.IdUf, x.DescricaoCidade, 1, 35, "Sao Paulo"),
            x => AssertCidade(x.IdCidade, x.IdUf, x.DescricaoCidade, 2, 33, "Rio de Janeiro"));
        repository.Verify(x => x.ListarCidadesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarCidadesAsync_QuandoNaoExistemRegistros_DeveRetornarListaVazia()
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarCidadesAsync(It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await service.ListarCidadesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ListarCidadesAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarCidadesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ListarCidadesAsync());

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao listar cidades", expected);
    }

    [Fact]
    public async Task ObterCidadePorIdAsync_QuandoExiste_DeveRetornarListaComUmItemMapeado()
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ObterCidadePorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarModel(1, 35, "Sao Paulo"));

        var result = await service.ObterCidadePorIdAsync(1);

        var item = Assert.Single(result);
        AssertCidade(item.IdCidade, item.IdUf, item.DescricaoCidade, 1, 35, "Sao Paulo");
        repository.Verify(x => x.ObterCidadePorIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterCidadePorIdAsync_QuandoNaoExiste_DeveLogarELancarInvalidOperationException()
    {
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterCidadePorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CidadeModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterCidadePorIdAsync(999));

        Assert.Equal("Cidade não encontrada", exception.Message);
        VerificarLogErro(logger, "Erro ao obter cidade com ID 999", exception);
    }

    [Fact]
    public async Task ObterCidadePorIdAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterCidadePorIdAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterCidadePorIdAsync(1));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao obter cidade com ID 1", expected);
    }

    [Fact]
    public async Task ObterCidadesPorUfAsync_QuandoExistemRegistros_DeveFiltrarEMapear()
    {
        var cidades = new List<CidadeModel>
        {
            CriarModel(1, 35, "Sao Paulo"),
            CriarModel(2, 35, "Campinas")
        };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarCidadesPorUfAsync(35, It.IsAny<CancellationToken>()))
            .ReturnsAsync(cidades);

        var result = await service.ObterCidadesPorUfAsync(35);

        Assert.Collection(result,
            x => AssertCidade(x.IdCidade, x.IdUf, x.DescricaoCidade, 1, 35, "Sao Paulo"),
            x => AssertCidade(x.IdCidade, x.IdUf, x.DescricaoCidade, 2, 35, "Campinas"));
        repository.Verify(x => x.ListarCidadesPorUfAsync(35, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ObterCidadesPorUfAsync_QuandoNaoExistemRegistros_DeveRetornarListaVazia()
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ListarCidadesPorUfAsync(35, It.IsAny<CancellationToken>())).ReturnsAsync([]);

        var result = await service.ObterCidadesPorUfAsync(35);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ObterCidadesPorUfAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao filtrar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarCidadesPorUfAsync(35, It.IsAny<CancellationToken>()))
            .ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterCidadesPorUfAsync(35));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao obter cidades para UF com ID 35", expected);
    }

    [Fact]
    public async Task IncluirCidadeAsync_DeveRepassarRequestEMapearResposta()
    {
        var request = new CidadeModelRequest { IdCidade = 999, IdUf = 35, DescricaoCidade = "Sao Paulo" };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.IncluirCidadeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarModel(1, 35, "Sao Paulo"));

        var result = await service.IncluirCidadeAsync(request);

        AssertCidade(result.IdCidade, result.IdUf, result.DescricaoCidade, 1, 35, "Sao Paulo");
        repository.Verify(x => x.IncluirCidadeAsync(
            It.Is<CidadeModelRequest>(x => ReferenceEquals(x, request)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IncluirCidadeAsync_ComDescricaoNula_DeveMapearDescricaoNula()
    {
        var request = new CidadeModelRequest { IdUf = 35, DescricaoCidade = null };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.IncluirCidadeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarModel(1, 35, null));

        var result = await service.IncluirCidadeAsync(request);

        Assert.Equal(1, result.IdCidade);
        Assert.Equal(35, result.IdUf);
        Assert.Null(result.DescricaoCidade);
    }

    [Fact]
    public async Task IncluirCidadeAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var request = new CidadeModelRequest { IdUf = 35, DescricaoCidade = "Sao Paulo" };
        var expected = new InvalidOperationException("Falha ao incluir");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.IncluirCidadeAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.IncluirCidadeAsync(request));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao incluir cidade", expected);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoExiste_DeveRepassarRequestEMapearResposta()
    {
        var request = new CidadeModelRequest { IdCidade = 1, IdUf = 33, DescricaoCidade = "Rio de Janeiro" };
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.AlterarCidadeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CriarModel(1, 33, "Rio de Janeiro"));

        var result = await service.AlterarCidadeAsync(request);

        AssertCidade(result.IdCidade, result.IdUf, result.DescricaoCidade, 1, 33, "Rio de Janeiro");
        repository.Verify(x => x.AlterarCidadeAsync(
            It.Is<CidadeModelRequest>(x => ReferenceEquals(x, request)),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoNaoExiste_DeveLogarELancarInvalidOperationException()
    {
        var request = new CidadeModelRequest { IdCidade = 999, IdUf = 35, DescricaoCidade = "Inexistente" };
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarCidadeAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CidadeModel?)null);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AlterarCidadeAsync(request));

        Assert.Equal("Cidade não encontrada para alteração", exception.Message);
        VerificarLogErro(logger, "Erro ao alterar cidade com ID 999", exception);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var request = new CidadeModelRequest { IdCidade = 1 };
        var expected = new InvalidOperationException("Falha ao alterar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarCidadeAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AlterarCidadeAsync(request));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao alterar cidade com ID 1", expected);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoExiste_DeveRetornarTrue()
    {
        var (service, repository, _) = CriarServico();
        repository.Setup(x => x.ApagarCidadeAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await service.ApagarCidadeAsync(1);

        Assert.True(result);
        repository.Verify(x => x.ApagarCidadeAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoNaoExiste_DeveLogarELancarInvalidOperationException()
    {
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarCidadeAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApagarCidadeAsync(999));

        Assert.Equal("Cidade não encontrada para exclusão", exception.Message);
        VerificarLogErro(logger, "Erro ao apagar cidade com ID 999", exception);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoRepositorioFalha_DeveLogarERelancar()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarCidadeAsync(1, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApagarCidadeAsync(1));

        Assert.Same(expected, exception);
        VerificarLogErro(logger, "Erro ao apagar cidade com ID 1", expected);
    }

    private static (
        CidadeBusiness Service,
        Mock<ICidadeRepository> Repository,
        Mock<ILogger<CidadeBusiness>> Logger) CriarServico()
    {
        var repository = new Mock<ICidadeRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<CidadeBusiness>>();
        return (new CidadeBusiness(logger.Object, repository.Object), repository, logger);
    }

    private static CidadeModel CriarModel(int idCidade, int idUf, string? descricao) => new()
    {
        IdCidade = idCidade,
        IdUf = idUf,
        DescricaoCidade = descricao
    };

    private static void AssertCidade(
        int idCidade, int idUf, string? descricao, int idEsperado, int ufEsperada, string? descricaoEsperada)
    {
        Assert.Equal(idEsperado, idCidade);
        Assert.Equal(ufEsperada, idUf);
        Assert.Equal(descricaoEsperada, descricao);
    }

    private static void VerificarLogErro(
        Mock<ILogger<CidadeBusiness>> logger, string mensagem, Exception exception)
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
