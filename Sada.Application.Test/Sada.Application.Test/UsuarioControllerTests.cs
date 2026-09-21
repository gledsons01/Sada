using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;
using Sada.Application.Services;

namespace Sada.Application.Test;

public sealed class UsuarioControllerTests
{
    private const string MensagemErro = "Ocorreu um erro ao processar a solicitacao.";

    [Fact]
    public async Task ListarUsuariosAsync_QuandoExistemRegistros_DeveRetornarOkComMesmaLista()
    {
        var expected = new List<UsuarioModelResponse> { CriarResponse(1), CriarResponse(2) };
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ListUsuariosAsync()).ReturnsAsync(expected);

        var result = await controller.ListarUsuariosAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ListUsuariosAsync(), Times.Once);
    }

    [Fact]
    public async Task ListarUsuariosAsync_QuandoListaVazia_DeveRetornarOkComListaVazia()
    {
        var expected = new List<UsuarioModelResponse>();
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ListUsuariosAsync()).ReturnsAsync(expected);

        var result = await controller.ListarUsuariosAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Empty(Assert.IsType<List<UsuarioModelResponse>>(ok.Value));
    }

    [Fact]
    public async Task ListarUsuariosAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (controller, business, _, logger) = CriarController();
        business.Setup(x => x.ListUsuariosAsync()).ThrowsAsync(expected);

        var result = await controller.ListarUsuariosAsync();

        AssertStatus(result, 500, MensagemErro);
        VerificarLogErro(logger, "Erro ao listar usuarios", expected);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoExiste_DeveRetornarOkComResposta()
    {
        var expected = CriarResponse(7);
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ObterUsuarioPorIdAsync(7)).ReturnsAsync(expected);

        var result = await controller.ObterUsuarioPorIdAsync(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.ObterUsuarioPorIdAsync(7), Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoNaoExiste_DeveRetornar404()
    {
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ObterUsuarioPorIdAsync(999)).ReturnsAsync((UsuarioModelResponse?)null);

        var result = await controller.ObterUsuarioPorIdAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 999 nao encontrado.", notFound.Value);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (controller, business, _, logger) = CriarController();
        business.Setup(x => x.ObterUsuarioPorIdAsync(7)).ThrowsAsync(expected);

        var result = await controller.ObterUsuarioPorIdAsync(7);

        AssertStatus(result, 500, MensagemErro);
        VerificarLogErro(logger, "Erro ao obter usuario com ID 7.", expected);
    }

    [Fact]
    public async Task CadastrarUsuarioAsync_QuandoSucesso_DeveRepassarRequestERetornarOk()
    {
        var request = CriarRequest();
        var expected = CriarResponse(10);
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.CadastrarUsuarioAsync(request)).ReturnsAsync(expected);

        var result = await controller.CadastrarUsuarioAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.CadastrarUsuarioAsync(
            It.Is<UsuarioModelRequest>(model => ReferenceEquals(model, request))), Times.Once);
    }

    [Fact]
    public async Task CadastrarUsuarioAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var request = CriarRequest();
        var expected = new InvalidOperationException("Falha ao cadastrar");
        var (controller, business, _, logger) = CriarController();
        business.Setup(x => x.CadastrarUsuarioAsync(request)).ThrowsAsync(expected);

        var result = await controller.CadastrarUsuarioAsync(request);

        AssertStatus(result, 500, MensagemErro);
        VerificarLogErro(logger, "Erro ao cadastrar usuario", expected);
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoExiste_DeveRepassarRequestERetornarOk()
    {
        var request = CriarRequest(7);
        var expected = CriarResponse(7);
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.AlterarUsuarioAsync(request)).ReturnsAsync(expected);

        var result = await controller.AlterarUsuarioAsync(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        business.Verify(x => x.AlterarUsuarioAsync(
            It.Is<UsuarioModelRequest>(model => ReferenceEquals(model, request))), Times.Once);
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoNaoExiste_DeveRetornar404()
    {
        var request = CriarRequest(999);
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.AlterarUsuarioAsync(request)).ReturnsAsync((UsuarioModelResponse?)null);

        var result = await controller.AlterarUsuarioAsync(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 999 nao encontrado.", notFound.Value);
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var request = CriarRequest(7);
        var expected = new InvalidOperationException("Falha ao alterar");
        var (controller, business, _, logger) = CriarController();
        business.Setup(x => x.AlterarUsuarioAsync(request)).ThrowsAsync(expected);

        var result = await controller.AlterarUsuarioAsync(request);

        AssertStatus(result, 500, MensagemErro);
        VerificarLogErro(logger, "Erro ao alterar usuario com ID 7", expected);
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoExiste_DeveRetornarOkComMensagem()
    {
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ApagarUsuarioAsync(7)).ReturnsAsync(true);

        var result = await controller.ApagarUsuarioAsync(7);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Usuario com ID 7 apagado com sucesso.", ok.Value);
        business.Verify(x => x.ApagarUsuarioAsync(7), Times.Once);
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoNaoExiste_DeveRetornar404()
    {
        var (controller, business, _, _) = CriarController();
        business.Setup(x => x.ApagarUsuarioAsync(999)).ReturnsAsync(false);

        var result = await controller.ApagarUsuarioAsync(999);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 999 nao encontrado.", notFound.Value);
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoBusinessFalha_DeveRetornar500ELogarErro()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (controller, business, _, logger) = CriarController();
        business.Setup(x => x.ApagarUsuarioAsync(7)).ThrowsAsync(expected);

        var result = await controller.ApagarUsuarioAsync(7);

        AssertStatus(result, 500, MensagemErro);
        VerificarLogErro(logger, "Erro ao apagar usuario com ID: 7", expected);
    }

    [Theory]
    [InlineData(null, "senha")]
    [InlineData("", "senha")]
    [InlineData("email@teste.com", null)]
    [InlineData("email@teste.com", "")]
    public async Task LoginUsuarioAsync_ComEmailOuSenhaAusentes_DeveRetornar400SemChamarDependencias(
        string? email, string? senha)
    {
        var (controller, business, jwt, _) = CriarController();

        var result = await controller.LoginUsuarioAsync(email!, senha!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("LoginUsuario() - Email e senha sao obrigatorios.", badRequest.Value);
        business.Verify(x => x.LoginUsuarioAsync(It.IsAny<LoginModelRequest>()), Times.Never);
        jwt.Verify(x => x.PreencherTokens(It.IsAny<UsuarioModelResponse>()), Times.Never);
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoSucesso_DeveCriarRequestPreencherTokensERetornarOk()
    {
        var expected = CriarResponse(7);
        var (controller, business, jwt, _) = CriarController();
        business.Setup(x => x.LoginUsuarioAsync(It.Is<LoginModelRequest>(
                model => model.Login == "email@teste.com" && model.Senha == "senha")))
            .ReturnsAsync(expected);
        jwt.Setup(x => x.PreencherTokens(expected))
            .Callback<UsuarioModelResponse>(usuario => usuario.Token = "jwt");

        var result = await controller.LoginUsuarioAsync("email@teste.com", "senha");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Equal("jwt", expected.Token);
        business.Verify(x => x.LoginUsuarioAsync(It.IsAny<LoginModelRequest>()), Times.Once);
        jwt.Verify(x => x.PreencherTokens(expected), Times.Once);
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoUsuarioNaoExiste_DeveRetornar404SemPreencherTokens()
    {
        var (controller, business, jwt, _) = CriarController();
        business.Setup(x => x.LoginUsuarioAsync(It.IsAny<LoginModelRequest>()))
            .ReturnsAsync((UsuarioModelResponse)null!);

        var result = await controller.LoginUsuarioAsync("inexistente", "senha");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario nao encontrado.", notFound.Value);
        jwt.Verify(x => x.PreencherTokens(It.IsAny<UsuarioModelResponse>()), Times.Never);
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoBusinessFalha_DeveRetornar500SemPreencherTokensELogarErro()
    {
        var expected = new InvalidOperationException("Falha no login");
        var (controller, business, jwt, logger) = CriarController();
        business.Setup(x => x.LoginUsuarioAsync(It.IsAny<LoginModelRequest>())).ThrowsAsync(expected);

        var result = await controller.LoginUsuarioAsync("email@teste.com", "senha");

        AssertStatus(result, 500, MensagemErro);
        jwt.Verify(x => x.PreencherTokens(It.IsAny<UsuarioModelResponse>()), Times.Never);
        VerificarLogErro(logger, "Erro ao fazer login do usuario", expected);
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoServicoJwtFalha_DeveRetornar500ELogarErro()
    {
        var usuario = CriarResponse(7);
        var expected = new InvalidOperationException("Falha ao gerar token");
        var (controller, business, jwt, logger) = CriarController();
        business.Setup(x => x.LoginUsuarioAsync(It.IsAny<LoginModelRequest>())).ReturnsAsync(usuario);
        jwt.Setup(x => x.PreencherTokens(usuario)).Throws(expected);

        var result = await controller.LoginUsuarioAsync("email@teste.com", "senha");

        AssertStatus(result, 500, MensagemErro);
        jwt.Verify(x => x.PreencherTokens(usuario), Times.Once);
        VerificarLogErro(logger, "Erro ao fazer login do usuario", expected);
    }

    private static (
        UsuarioController Controller,
        Mock<IUsuario> Business,
        Mock<IJwtTokenService> Jwt,
        Mock<ILogger<UsuarioController>> Logger) CriarController()
    {
        var business = new Mock<IUsuario>(MockBehavior.Strict);
        var jwt = new Mock<IJwtTokenService>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UsuarioController>>();
        return (new UsuarioController(logger.Object, business.Object, jwt.Object), business, jwt, logger);
    }

    private static UsuarioModelRequest CriarRequest(int? id = null) => new()
    {
        IdUsuario = id, NomeUsuario = "Nome Teste", Login = "usuario", Senha = "senha",
        Endereco = "Rua Teste", NumeroEndereco = "100", Bairro = "Centro",
        IdUf = 35, IdCidade = 1, NomeSocial = "Social", IdSexo = 2, EMail = "email@teste.com"
    };

    private static UsuarioModelResponse CriarResponse(int id) => new()
    {
        IdUsuario = id,
        NomeUsuario = "Nome Teste",
        Login = "usuario",
        EMail = "email@teste.com",
        blnRetorno = true
    };

    private static void AssertStatus(IActionResult result, int statusCode, string mensagem)
    {
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(statusCode, status.StatusCode);
        Assert.Equal(mensagem, status.Value);
    }

    private static void VerificarLogErro(
        Mock<ILogger<UsuarioController>> logger, string mensagem, Exception exception)
    {
        logger.Verify(x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() != null && state.ToString()!.Contains(mensagem)),
                It.Is<Exception?>(value => ReferenceEquals(value, exception)),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
