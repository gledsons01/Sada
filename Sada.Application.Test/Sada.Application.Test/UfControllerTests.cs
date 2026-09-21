using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;
using Sada.Application.Services;

namespace Sada.Application.Test;

public sealed class UfControllerTests
{
    private const string MensagemErro = "Ocorreu um erro ao processar a solicitacao.";

    [Fact]
    public async Task ListarUfAsync_QuandoExistemRegistros_DeveRetornarOkComListaELogarSucesso()
    {
        var expected = new List<UfModelResponse>
        {
            CriarResponse(33, "RJ", "Rio de Janeiro"),
            CriarResponse(35, "SP", "Sao Paulo")
        };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarUfsAsync()).ReturnsAsync(expected);

        var result = await controller.ListarUfAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ListarUfsAsync(), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de UFs efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarUfAsync_QuandoListaVazia_DeveRetornarOkComListaVazia()
    {
        var expected = new List<UfModelResponse>();
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarUfsAsync()).ReturnsAsync(expected);

        var result = await controller.ListarUfAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Empty(Assert.IsType<List<UfModelResponse>>(ok.Value));
        VerificarLog(logger, LogLevel.Information, "Listagem de UFs efetuada com sucesso.");
    }

    [Fact]
    public async Task ListarUfAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ListarUfsAsync()).ThrowsAsync(expected);

        var result = await controller.ListarUfAsync();

        AssertStatus(result, 500, "Ocorreu um erro ao processar a solicitacao de listagem de UFs.");
        VerificarLog(logger, LogLevel.Error, "Erro ao listar as UFs.", expected);
    }

    [Fact]
    public async Task ObterUfAsync_QuandoExiste_DeveRetornarOkComResultadoELogarSucesso()
    {
        var expected = new List<UfModelResponse> { CriarResponse(35, "SP", "Sao Paulo") };
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterUfPorIdAsync(35)).ReturnsAsync(expected);

        var result = await controller.ObterUfAsync(35);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ObterUfPorIdAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Information, "UF encontrada com sucesso através do Id:");
    }

    [Fact]
    public async Task ObterUfAsync_QuandoBusinessRetornaNulo_DeveRetornar404ELogarAviso()
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterUfPorIdAsync(999)).ReturnsAsync((List<UfModelResponse>)null!);

        var result = await controller.ObterUfAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("UF com ID 999 não encontrada.", notFound.Value);
        VerificarLog(logger, LogLevel.Warning, "UF não encontrada ID: 999");
    }

    [Fact]
    public async Task ObterUfAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ObterUfPorIdAsync(35)).ThrowsAsync(expected);

        var result = await controller.ObterUfAsync(35);

        AssertStatus(result, 500, MensagemErro);
        VerificarLog(logger, LogLevel.Error, "Erro ao obter UF com ID 35", expected);
    }

    [Fact]
    public async Task CadastrarUfAsync_QuandoSucesso_DeveRepassarRequestRetornarOkELogarSucesso()
    {
        var request = new UfModelRequest { IdUf = 999, SiglaUf = "SP", Sigla = "Sao Paulo" };
        var expected = CriarResponse(35, "SP", "Sao Paulo");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.IncluirUfAsync(request)).ReturnsAsync(expected);

        var result = await controller.CadastrarUfAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.IncluirUfAsync(
            It.Is<UfModelRequest>(model => ReferenceEquals(model, request))), Times.Once);
        VerificarLog(logger, LogLevel.Information, "UF cadastrada com sucesso:");
    }

    [Fact]
    public async Task CadastrarUfAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var request = new UfModelRequest { SiglaUf = "SP", Sigla = "Sao Paulo" };
        var expected = new InvalidOperationException("Falha ao cadastrar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.IncluirUfAsync(request)).ThrowsAsync(expected);

        var result = await controller.CadastrarUfAsync(request);

        AssertStatus(result, 500, MensagemErro);
        VerificarLog(logger, LogLevel.Error, "Erro ao cadastrar UF", expected);
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoSucesso_DeveSobrescreverIdRepassarRequestRetornarOkELogar()
    {
        var request = new UfModelRequest { IdUf = 999, SiglaUf = "RJ", Sigla = "Rio de Janeiro" };
        var expected = CriarResponse(33, "RJ", "Rio de Janeiro");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarUfAsync(
                It.Is<UfModelRequest>(model => ReferenceEquals(model, request) && model.IdUf == 33)))
            .ReturnsAsync(expected);

        var result = await controller.AlterarUfAsync(33, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Equal(33, request.IdUf);
        business.Verify(x => x.AlterarUfAsync(It.IsAny<UfModelRequest>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "UF alterada com sucesso:");
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoBusinessFalha_DeveManterIdDaRotaRetornar500ELogarErro()
    {
        var request = new UfModelRequest { IdUf = 999, SiglaUf = "RJ", Sigla = "Rio de Janeiro" };
        var expected = new InvalidOperationException("Falha ao alterar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.AlterarUfAsync(
                It.Is<UfModelRequest>(model => ReferenceEquals(model, request) && model.IdUf == 33)))
            .ThrowsAsync(expected);

        var result = await controller.AlterarUfAsync(33, request);

        AssertStatus(result, 500, MensagemErro);
        Assert.Equal(33, request.IdUf);
        VerificarLog(logger, LogLevel.Error, "Erro ao alterar UF", expected);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ApagarUfAsync_DeveRetornarOkComResultadoDoBusinessELogarSucesso(bool resultado)
    {
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ApagarUfAsync(35)).ReturnsAsync(resultado);

        var actionResult = await controller.ApagarUfAsync(35);

        var ok = Assert.IsType<OkObjectResult>(actionResult);
        Assert.Equal(resultado, ok.Value);
        business.Verify(x => x.ApagarUfAsync(35), Times.Once);
        VerificarLog(logger, LogLevel.Information, $"UF apagada com sucesso: {resultado}");
    }

    [Fact]
    public async Task ApagarUfAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (controller, business, logger) = CriarController();
        business.Setup(x => x.ApagarUfAsync(35)).ThrowsAsync(expected);

        var result = await controller.ApagarUfAsync(35);

        AssertStatus(result, 500, MensagemErro);
        VerificarLog(logger, LogLevel.Error, "Erro ao apagar UF", expected);
    }

    [Fact]
    public void Index_DeveRetornarViewResult()
    {
        var (controller, _, _) = CriarController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    private static (
        UfController Controller,
        Mock<IUf> Business,
        Mock<ILogger<UfController>> Logger) CriarController()
    {
        var business = new Mock<IUf>(MockBehavior.Strict);
        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UfController>>();
        return (new UfController(business.Object, jwt.Object, logger.Object), business, logger);
    }

    private static UfModelResponse CriarResponse(int idUf, string? siglaUf, string? sigla) => new()
    {
        IdUf = idUf,
        SiglaUf = siglaUf,
        Sigla = sigla
    };

    private static void AssertStatus(IActionResult result, int statusCode, string mensagem)
    {
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, status.StatusCode);
        Assert.Equal(mensagem, status.Value);
    }

    private static void VerificarLog(
        Mock<ILogger<UfController>> logger,
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
