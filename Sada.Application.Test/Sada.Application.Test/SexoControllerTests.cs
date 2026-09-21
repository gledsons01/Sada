using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;

namespace Sada.Application.Test;

public sealed class SexoControllerTests
{
    private const string MensagemErro = "Ocorreu um erro ao processar a solicitacao.";

    [Fact]
    public async Task ListarSexoAsync_QuandoSucesso_DeveRetornarOkComListaERegistrarInformacao()
    {
        var expected = new List<SexoModelResponse>
        {
            CriarResponse(1, "Feminino", "F"),
            CriarResponse(2, "Masculino", "M")
        };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarSexosAsync()).ReturnsAsync(expected);

        var result = await controller.ListarSexoAsync();

        var ok = Assert.IsType<OkObjectResult>(result); 
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ListarSexosAsync(), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de Sexo efetuada com sucesso");
    }

    [Fact]
    public async Task ListarSexoAsync_QuandoBusinessLancaExcecao_DeveRetornar500ERegistrarErro()
    {
        var exception = new InvalidOperationException("Falha simulada");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarSexosAsync()).ThrowsAsync(exception);

        var result = await controller.ListarSexoAsync();

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal(MensagemErro, status.Value);
        business.Verify(x => x.ListarSexosAsync(), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao listar usuarios", exception);
    }

    [Fact]
    public async Task CapturarSexoAsync_QuandoExiste_DeveRetornarOkComSexoERegistrarInformacao()
    {
        const int id = 2;
        var expected = CriarResponse(id, "Masculino", "M");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterSexoPorIdAsync(id)).ReturnsAsync(expected);

        var result = await controller.CapturarSexoAsync(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ObterSexoPorIdAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Tipo de sexo encontrado com sucesso");
    }

    [Fact]
    public async Task CapturarSexoAsync_QuandoNaoExiste_DeveRetornar404ERegistrarAviso()
    {
        const int id = 999;
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterSexoPorIdAsync(id))
            .ReturnsAsync((SexoModelResponse)null!);

        var result = await controller.CapturarSexoAsync(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 999 nao encontrado.", notFound.Value);
        business.Verify(x => x.ObterSexoPorIdAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Warning, "Sexo não encontrado ID: 999");
    }

    [Fact]
    public async Task CapturarSexoAsync_QuandoBusinessLancaExcecao_DeveRetornar500ERegistrarErro()
    {
        const int id = 3;
        var exception = new KeyNotFoundException("Não encontrado");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterSexoPorIdAsync(id)).ThrowsAsync(exception);

        var result = await controller.CapturarSexoAsync(id);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal(MensagemErro, status.Value);
        business.Verify(x => x.ObterSexoPorIdAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao obter tipo de sexo com ID 3", exception);
    }

    [Fact]
    public async Task CadastrarSexoAsync_QuandoSucesso_DeveRepassarRequestRetornarOkERegistrarInformacao()
    {
        var request = new SexoModelRequest { Descricao = "Feminino", Sigla = "F" };
        var expected = CriarResponse(1, request.Descricao, request.Sigla);
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.CadastrarSexoAsync(request)).ReturnsAsync(expected);

        var result = await controller.CadastrarSexoAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(
            x => x.CadastrarSexoAsync(
                It.Is<SexoModelRequest>(model => ReferenceEquals(model, request))),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Cadasdo do Tipo de Sexo efetuado com sucesso");
    }

    [Fact]
    public async Task CadastrarSexoAsync_QuandoBusinessLancaExcecao_DeveRetornar500ERegistrarErro()
    {
        var request = new SexoModelRequest { Descricao = "Feminino", Sigla = "F" };
        var exception = new InvalidOperationException("Falha simulada");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.CadastrarSexoAsync(request)).ThrowsAsync(exception);

        var result = await controller.CadastrarSexoAsync(request);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal(MensagemErro, status.Value);
        business.Verify(x => x.CadastrarSexoAsync(request), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao cadastrar tipo de sexo.", exception);
    }

    [Fact]
    public async Task AlterSexoAsync_QuandoExiste_DeveRepassarRequestRetornarOkERegistrarInformacao()
    {
        var request = new SexoModelRequest
        {
            IdSexo = 2,
            Descricao = "Masculino",
            Sigla = "M"
        };
        var expected = CriarResponse(request.IdSexo, request.Descricao, request.Sigla);
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarSexoAsync(request)).ReturnsAsync(expected);

        var result = await controller.AlterSexoAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(
            x => x.AlterarSexoAsync(
                It.Is<SexoModelRequest>(model => ReferenceEquals(model, request))),
            Times.Once);
        VerificarLog(logger, LogLevel.Information, "Sexo alterado com sucesso. M.");
    }

    [Fact]
    public async Task AlterSexoAsync_QuandoNaoExiste_DeveRetornar404ERegistrarAviso()
    {
        var request = new SexoModelRequest
        {
            IdSexo = 999,
            Descricao = "Inexistente",
            Sigla = "IN"
        };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarSexoAsync(request))
            .ReturnsAsync((SexoModelResponse)null!);

        var result = await controller.AlterSexoAsync(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Sexo não encontrado. IN.", notFound.Value);
        business.Verify(x => x.AlterarSexoAsync(request), Times.Once);
        VerificarLog(logger, LogLevel.Warning, "Sexo não encontrado. IN.");
    }

    [Fact]
    public async Task AlterSexoAsync_QuandoBusinessLancaExcecao_DeveRetornar500ERegistrarErro()
    {
        var request = new SexoModelRequest { IdSexo = 3, Sigla = "NI" };
        var exception = new KeyNotFoundException("Não encontrado");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarSexoAsync(request)).ThrowsAsync(exception);

        var result = await controller.AlterSexoAsync(request);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal(MensagemErro, status.Value);
        business.Verify(x => x.AlterarSexoAsync(request), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao alterar tipo de sexo.", exception);
    }

    [Fact]
    public async Task ApagarSexoAsync_QuandoExiste_DeveRetornarOkTrueERegistrarInformacao()
    {
        const int id = 2;
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ExcluirSexoAsync(id)).ReturnsAsync(true);

        var result = await controller.ApagarSexoAsync(id);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(true, ok.Value);
        business.Verify(x => x.ExcluirSexoAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Tipo de Sexo excluído com suscesso. 2.");
    }

    [Fact]
    public async Task ApagarSexoAsync_QuandoBusinessRetornaFalse_DeveRetornar404ERegistrarAviso()
    {
        const int id = 999;
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ExcluirSexoAsync(id)).ReturnsAsync(false);

        var result = await controller.ApagarSexoAsync(id);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Tipo de Sexo não encontrado. 999.", notFound.Value);
        business.Verify(x => x.ExcluirSexoAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Warning, "Tipo de sexo não encontrado. 999.");
    }

    [Fact]
    public async Task ApagarSexoAsync_QuandoBusinessLancaExcecao_DeveRetornar500ERegistrarErro()
    {
        const int id = 3;
        var exception = new KeyNotFoundException("Não encontrado");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ExcluirSexoAsync(id)).ThrowsAsync(exception);

        var result = await controller.ApagarSexoAsync(id);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal(MensagemErro, status.Value);
        business.Verify(x => x.ExcluirSexoAsync(id), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao apagar sexo tipo de sexo.", exception);
    }

    private static (
        SexoController Controller,
        Mock<ISexo> Business,
        Mock<ILogger<SexoController>> Logger) CriarController()
    {
        var business = new Mock<ISexo>(MockBehavior.Strict);
        var logger = new Mock<ILogger<SexoController>>();

        return (new SexoController(logger.Object, business.Object), business, logger);
    }

    private static SexoModelResponse CriarResponse(
        int id,
        string? descricao,
        string? sigla)
    {
        return new SexoModelResponse
        {
            IdSexo = id,
            Descricao = descricao,
            Sigla = sigla
        };
    }

    private static void VerificarLog(
        Mock<ILogger<SexoController>> logger,
        LogLevel level,
        string mensagem,
        Exception? exception = null)
    {
        if (exception is null)
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
            return;
        }

        logger.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() != null &&
                    state.ToString()!.Contains(mensagem)),
                It.Is<Exception?>(value => ReferenceEquals(value, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
