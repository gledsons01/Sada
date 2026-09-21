using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using SexoBusiness = Sada.Api.Business.Sexo;

namespace Sada.Api.Business.Test;

public sealed class SexoTests
{
    [Fact]
    public void Sexo_DeveImplementarInterfaceISexo()
    {
        var (service, _, _) = CriarServico();

        Assert.IsAssignableFrom<ISexo>(service);
    }

    [Fact]
    public async Task ListarSexosAsync_QuandoExistemRegistros_DeveRetornarListaERegistrarQuantidade()
    {
        var expected = new List<SexoModelResponse>
        {
            CriarResponse(1, "Feminino", "F"),
            CriarResponse(2, "Masculino", "M")
        };
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ListarSexosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await service.ListarSexosAsync();

        Assert.Same(expected, result);
        repository.Verify(
            x => x.ListarSexosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de sexos realizada. Quantidade: 2");
    }

    [Fact]
    public async Task ListarSexosAsync_QuandoNaoExistemRegistros_DeveRetornarListaVaziaERegistrarZero()
    {
        var expected = new List<SexoModelResponse>();
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ListarSexosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await service.ListarSexosAsync();

        Assert.Same(expected, result);
        Assert.Empty(result);
        repository.Verify(
            x => x.ListarSexosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de sexos realizada. Quantidade: 0");
    }

    [Fact]
    public async Task ObterSexoPorIdAsync_QuandoExiste_DeveRetornarRespostaERegistrarSucesso()
    {
        const int idSexo = 2;
        var expected = CriarResponse(idSexo, "Masculino", "M");
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ObterSexoPorIdAsync(idSexo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await service.ObterSexoPorIdAsync(idSexo);

        Assert.Same(expected, result);
        repository.Verify(
            x => x.ObterSexoPorIdAsync(idSexo, It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Sexo com ID 2 obtido com sucesso.");
    }

    [Fact]
    public async Task ObterSexoPorIdAsync_QuandoNaoExiste_DeveRegistrarAvisoELancarKeyNotFoundException()
    {
        const int idSexo = 999;
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ObterSexoPorIdAsync(idSexo, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SexoModelResponse?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.ObterSexoPorIdAsync(idSexo));

        Assert.Equal("Sexo com ID 999 não encontrado.", exception.Message);
        repository.Verify(
            x => x.ObterSexoPorIdAsync(idSexo, It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Warning, "Sexo com ID 999 não encontrado.");
    }

    [Fact]
    public async Task CadastrarSexoAsync_DeveRepassarRequestRetornarRespostaERegistrarSucesso()
    {
        var request = new SexoModelRequest
        {
            Descricao = "Feminino",
            Sigla = "F"
        };
        var expected = CriarResponse(1, request.Descricao, request.Sigla);
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.IncluirSexoAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await service.CadastrarSexoAsync(request);

        Assert.Same(expected, result);
        repository.Verify(
            x => x.IncluirSexoAsync(
                It.Is<SexoModelRequest>(model => ReferenceEquals(model, request)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Cadastro de sexo realizado com sucesso. ID: 1");
    }

    [Fact]
    public async Task AlterarSexoAsync_QuandoExiste_DeveRepassarRequestRetornarRespostaERegistrarSucesso()
    {
        var request = new SexoModelRequest
        {
            IdSexo = 3,
            Descricao = "Nao informado",
            Sigla = "NI"
        };
        var expected = CriarResponse(request.IdSexo, request.Descricao, request.Sigla);
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.AlterarSexoAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await service.AlterarSexoAsync(request);

        Assert.Same(expected, result);
        repository.Verify(
            x => x.AlterarSexoAsync(
                It.Is<SexoModelRequest>(model => ReferenceEquals(model, request)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Sexo com ID 3 alterado com sucesso.");
    }

    [Fact]
    public async Task AlterarSexoAsync_QuandoNaoExiste_DeveRegistrarAvisoELancarKeyNotFoundException()
    {
        var request = new SexoModelRequest
        {
            IdSexo = 999,
            Descricao = "Inexistente",
            Sigla = "IN"
        };
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.AlterarSexoAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SexoModelResponse?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.AlterarSexoAsync(request));

        Assert.Equal(
            "Sexo com ID 999 não encontrado para alteração.",
            exception.Message);
        repository.Verify(
            x => x.AlterarSexoAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(
            logger,
            LogLevel.Warning,
            "Sexo com ID 999 não encontrado para alteração.");
    }

    [Fact]
    public async Task ExcluirSexoAsync_QuandoExiste_DeveRetornarTrueERegistrarSucesso()
    {
        const int idSexo = 4;
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ApagarSexoAsync(idSexo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await service.ExcluirSexoAsync(idSexo);

        Assert.True(result);
        repository.Verify(
            x => x.ApagarSexoAsync(idSexo, It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Sexo com ID 4 excluído com sucesso.");
    }

    [Fact]
    public async Task ExcluirSexoAsync_QuandoNaoExiste_DeveRegistrarAvisoELancarKeyNotFoundException()
    {
        const int idSexo = 999;
        var (service, repository, logger) = CriarServico();
        repository
            .Setup(x => x.ApagarSexoAsync(idSexo, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.ExcluirSexoAsync(idSexo));

        Assert.Equal(
            "Sexo com ID 999 não encontrado para exclusão.",
            exception.Message);
        repository.Verify(
            x => x.ApagarSexoAsync(idSexo, It.IsAny<CancellationToken>()),
            Times.Once);
        VerificarLog(
            logger,
            LogLevel.Warning,
            "Sexo com ID 999 não encontrado para exclusão.");
    }

    private static (
        SexoBusiness Service,
        Mock<ISexoRepository> Repository,
        Mock<ILogger<SexoBusiness>> Logger) CriarServico()
    {
        var repository = new Mock<ISexoRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<SexoBusiness>>();

        return (new SexoBusiness(logger.Object, repository.Object), repository, logger);
    }

    private static SexoModelResponse CriarResponse(
        int idSexo,
        string? descricao,
        string? sigla)
    {
        return new SexoModelResponse
        {
            IdSexo = idSexo,
            Descricao = descricao,
            Sigla = sigla
        };
    }

    private static void VerificarLog(
        Mock<ILogger<SexoBusiness>> logger,
        LogLevel level,
        string mensagem)
    {
        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() != null &&
                    state.ToString()!.Contains(mensagem)),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
