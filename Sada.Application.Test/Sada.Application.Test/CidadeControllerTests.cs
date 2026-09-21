using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;
using Sada.Application.Services;

namespace Sada.Application.Test;

public sealed class CidadeControllerTests
{
    [Fact]
    public async Task ListarCidadeAsync_QuandoExistemRegistros_DeveRetornarOkComListaELogarSucesso()
    {
        var expected = new List<CidadeModelResponse>
        {
            CriarResponse(1, 35, "Sao Paulo"),
            CriarResponse(2, 33, "Rio de Janeiro")
        };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarCidadesAsync()).ReturnsAsync(expected);

        var result = await controller.ListarCidadeAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ListarCidadesAsync(), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de cidades efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarCidadeAsync_QuandoListaVazia_DeveRetornarOkComListaVazia()
    {
        var expected = new List<CidadeModelResponse>();
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarCidadesAsync()).ReturnsAsync(expected);

        var result = await controller.ListarCidadeAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Empty(Assert.IsType<List<CidadeModelResponse>>(ok.Value));
        VerificarLog(logger, LogLevel.Information, "Listagem de cidades efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarCidadeAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarCidadesAsync()).ThrowsAsync(expected);

        var result = await controller.ListarCidadeAsync();

        AssertStatus(result, 500, "Ocorreu um erro ao processar a solicitacao de listagem de cidades.");
        VerificarLog(logger, LogLevel.Error, "Erro ao listar as cidades.", expected);
    }

    [Fact]
    public async Task ListarCidadesPorUfAsync_QuandoExistemRegistros_DeveRepassarIdRetornarOkELogarSucesso()
    {
        var expected = new List<CidadeModelResponse>
        {
            CriarResponse(1, 35, "Sao Paulo"),
            CriarResponse(2, 35, "Campinas")
        };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadesPorUfAsync(35)).ReturnsAsync(expected);

        var result = await controller.ListarCidadesPorUfAsync(35);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ObterCidadesPorUfAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de cidades por UF efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarCidadesPorUfAsync_QuandoListaVazia_DeveRetornarOkComListaVaziaELogarSucesso()
    {
        var expected = new List<CidadeModelResponse>();
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadesPorUfAsync(35)).ReturnsAsync(expected);

        var result = await controller.ListarCidadesPorUfAsync(35);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Empty(Assert.IsType<List<CidadeModelResponse>>(ok.Value));
        business.Verify(x => x.ObterCidadesPorUfAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de cidades por UF efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarCidadesPorUfAsync_QuandoBusinessRetornaNulo_DeveRetornarOkComValorNuloELogarSucesso()
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadesPorUfAsync(35))
            .ReturnsAsync((List<CidadeModelResponse>)null!);

        var result = await controller.ListarCidadesPorUfAsync(35);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Null(ok.Value);
        business.Verify(x => x.ObterCidadesPorUfAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de cidades por UF efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarCidadesPorUfAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao listar cidades por UF");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadesPorUfAsync(35)).ThrowsAsync(expected);

        var result = await controller.ListarCidadesPorUfAsync(35);

        AssertStatus(
            result,
            500,
            "Ocorreu um erro ao processar a solicitacao de listagem de cidades por UF.");
        business.Verify(x => x.ObterCidadesPorUfAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Error, "Erro ao listar cidades por UF.", expected);
    }

    [Fact]
    public async Task ObterCidadeAsync_QuandoExiste_DeveRetornarOkComResultadoELogarSucesso()
    {
        var expected = new List<CidadeModelResponse> { CriarResponse(1, 35, "Sao Paulo") };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadePorIdAsync(1)).ReturnsAsync(expected);

        var result = await controller.ObterCidadeAsync(1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ObterCidadePorIdAsync(1), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Cidade localizada 1.");
    }

    [Fact]
    public async Task ObterCidadeAsync_QuandoBusinessRetornaNulo_DeveRetornar404ELogarAviso()
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadePorIdAsync(999))
            .ReturnsAsync((List<CidadeModelResponse>)null!);

        var result = await controller.ObterCidadeAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Cidade não encontrada 999.", notFound.Value);
        VerificarLog(logger, LogLevel.Warning, "Cidade não encontrada 999.");
    }

    [Fact]
    public async Task ObterCidadeAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterCidadePorIdAsync(1)).ThrowsAsync(expected);

        var result = await controller.ObterCidadeAsync(1);

        AssertStatus(result, 500, "Ocorreu um erro ao processar a solicitacao de obter cidade.");
        VerificarLog(logger, LogLevel.Error, "Erro ao obter a cidade.", expected);
    }

    [Fact]
    public async Task CadastrarCidadeAsync_QuandoSucesso_DeveRepassarRequestRetornarOkELogarSucesso()
    {
        var request = CriarRequest(0, 35, "Sao Paulo");
        var expected = CriarResponse(1, 35, "Sao Paulo");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.IncluirCidadeAsync(request)).ReturnsAsync(expected);

        var result = await controller.CadastrarCidadeAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.IncluirCidadeAsync(
            It.Is<CidadeModelRequest>(model => ReferenceEquals(model, request))), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Cadasdo de cidade efetuado com sucesso.");
    }

    [Fact]
    public async Task CadastrarCidadeAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var request = CriarRequest(0, 35, "Sao Paulo");
        var expected = new InvalidOperationException("Falha ao cadastrar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.IncluirCidadeAsync(request)).ThrowsAsync(expected);

        var result = await controller.CadastrarCidadeAsync(request);

        AssertStatus(result, 500, "Ocorreu um erro ao processar a solicitacao de cadastar a cidade.");
        VerificarLog(logger, LogLevel.Error, "Erro ao cadastrar a cidade.", expected);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoExiste_DeveRepassarRequestRetornarOkELogarSucesso()
    {
        var request = CriarRequest(1, 33, "Rio de Janeiro");
        var expected = CriarResponse(1, 33, "Rio de Janeiro");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarCidadeAsync(request)).ReturnsAsync(expected);

        var result = await controller.AlterarCidadeAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.AlterarCidadeAsync(
            It.Is<CidadeModelRequest>(model => ReferenceEquals(model, request))), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Alteração de cidade efetuada com sucesso.");
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoBusinessRetornaNulo_DeveRetornar404ELogarAviso()
    {
        var request = CriarRequest(999, 35, "Inexistente");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarCidadeAsync(request)).ReturnsAsync((CidadeModelResponse)null!);

        var result = await controller.AlterarCidadeAsync(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Alteração de cidade não efetuada. Cidade não encontrada Inexistente.", notFound.Value);
        VerificarLog(logger, LogLevel.Warning, "Alteração de ciade não efetuada. Cidade não encontrada Inexistente.");
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var request = CriarRequest(1, 35, "Sao Paulo");
        var expected = new InvalidOperationException("Falha ao alterar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarCidadeAsync(request)).ThrowsAsync(expected);

        var result = await controller.AlterarCidadeAsync(request);

        AssertStatus(result, 500, "Ocorreu um erro ao processar a solicitacao de alterar dados da cidade.");
        VerificarLog(logger, LogLevel.Error, "Erro ao alterar dados da cidade.", expected);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoBusinessRetornaTrue_DeveRetornar404ConformeImplementacaoAtual()
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ApagarCidadeAsync(1)).ReturnsAsync(true);

        var result = await controller.ApagarCidadeAsync(1);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Exclusão de cidade não efetuada. 1.", notFound.Value);
        business.Verify(x => x.ApagarCidadeAsync(1), Times.Once);
        VerificarLog(logger, LogLevel.Warning, "Erro ao localizar a cidade 1.");
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoBusinessRetornaFalse_DeveRetornarOkFalseConformeImplementacaoAtual()
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ApagarCidadeAsync(999)).ReturnsAsync(false);

        var result = await controller.ApagarCidadeAsync(999);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(false, ok.Value);
        VerificarLog(logger, LogLevel.Information, "Exclusão de cidade efetuada com sucesso.");
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoBusinessFalha_DeveRetornar500ComIdELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ApagarCidadeAsync(7)).ThrowsAsync(expected);

        var result = await controller.ApagarCidadeAsync(7);

        AssertStatus(result, 500, "Erro ao excluir dados da cidade 7.");
        VerificarLog(logger, LogLevel.Error, "Erro ao excluir dados da cidade 7.", expected);
    }

    private static (
        CidadeController Controller,
        Mock<ICidade> Business,
        Mock<ILogger<CidadeController>> Logger) CriarController()
    {
        var business = new Mock<ICidade>(MockBehavior.Strict);
        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<CidadeController>>();
        return (new CidadeController(business.Object, jwt.Object, logger.Object), business, logger);
    }

    private static CidadeModelRequest CriarRequest(int idCidade, int idUf, string? descricao) => new()
    {
        IdCidade = idCidade,
        IdUf = idUf,
        DescricaoCidade = descricao
    };

    private static CidadeModelResponse CriarResponse(int idCidade, int idUf, string? descricao) => new()
    {
        IdCidade = idCidade,
        IdUf = idUf,
        DescricaoCidade = descricao
    };

    private static void AssertStatus(IActionResult result, int statusCode, string mensagem)
    {
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, status.StatusCode);
        Assert.Equal(mensagem, status.Value);
    }

    private static void VerificarLog(
        Mock<ILogger<CidadeController>> logger,
        LogLevel level,
        string mensagem,
        Exception? exception = null)
    {
        logger.Verify(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() != null && state.ToString()!.Contains(mensagem)),
                It.Is<Exception?>(value => exception == null || ReferenceEquals(value, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
