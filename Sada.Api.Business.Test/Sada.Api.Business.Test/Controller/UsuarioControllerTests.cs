using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;
using Sada.Application.Services;

namespace Sada.Api.Business.Test.Controller;

public sealed class UsuarioControllerTests
{
    private readonly Mock<ILogger<UsuarioController>> _loggerMock = new();
    private readonly Mock<IUsuario> _usuarioBusinessMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<ICacheService> _cacheServiceMock = new();

    [Fact]
    public async Task CadastrarUsuario_QuandoBusinessRetornaSucesso_DeveRetornarOkComUsuario()
    {
        var request = CriarRequest(0, "novo.usuario");
        var response = CriarResponse(1, request.Login);

        _usuarioBusinessMock
            .Setup(business => business.CadastrarUsuario(request))
            .ReturnsAsync(response);

        var controller = CriarController();

        var result = await controller.CadastrarUsuario(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        _usuarioBusinessMock.Verify(business => business.CadastrarUsuario(request), Times.Once);
    }

    [Fact]
    public async Task CadastrarUsuario_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        var request = CriarRequest(0, "usuario.erro");

        _usuarioBusinessMock
            .Setup(business => business.CadastrarUsuario(request))
            .ThrowsAsync(new InvalidOperationException("Falha ao cadastrar usuario"));

        var controller = CriarController();

        var result = await controller.CadastrarUsuario(request);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _usuarioBusinessMock.Verify(business => business.CadastrarUsuario(request), Times.Once);
    }

    [Fact]
    public async Task ListarUsuarios_QuandoBusinessRetornaUsuarios_DeveRetornarOkComLista()
    {
        var usuarios = new List<UsuarioModelResponse>
        {
            CriarResponse(1, "usuario1"),
            CriarResponse(2, "usuario2")
        };

        _usuarioBusinessMock
            .Setup(business => business.ListUsuarios())
            .ReturnsAsync(usuarios);

        var controller = CriarController();

        var result = await controller.ListarUsuarios();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(usuarios, ok.Value);
        _usuarioBusinessMock.Verify(business => business.ListUsuarios(), Times.Once);
    }

    [Fact]
    public async Task ListarUsuarios_QuandoBusinessRetornaListaVazia_DeveRetornarOkComListaVazia()
    {
        var usuarios = new List<UsuarioModelResponse>();

        _usuarioBusinessMock
            .Setup(business => business.ListUsuarios())
            .ReturnsAsync(usuarios);

        var controller = CriarController();

        var result = await controller.ListarUsuarios();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<UsuarioModelResponse>>(ok.Value);
        Assert.Empty(value);
        _usuarioBusinessMock.Verify(business => business.ListUsuarios(), Times.Once);
    }

    [Fact]
    public async Task ListarUsuarios_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        _usuarioBusinessMock
            .Setup(business => business.ListUsuarios())
            .ThrowsAsync(new InvalidOperationException("Falha ao listar usuarios"));

        var controller = CriarController();

        var result = await controller.ListarUsuarios();

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _usuarioBusinessMock.Verify(business => business.ListUsuarios(), Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorId_QuandoUsuarioExiste_DeveRetornarOkComUsuario()
    {
        const int idUsuario = 3;
        var response = CriarResponse(idUsuario, "usuario.consulta");

        _usuarioBusinessMock
            .Setup(business => business.ObterUsuarioPorId(idUsuario))
            .ReturnsAsync(response);

        var controller = CriarController();

        var result = await controller.ObterUsuarioPorId(idUsuario);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        _usuarioBusinessMock.Verify(business => business.ObterUsuarioPorId(idUsuario), Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorId_QuandoBusinessRetornaNull_DeveRetornarNotFound()
    {
        const int idUsuario = 99;

        _usuarioBusinessMock
            .Setup(business => business.ObterUsuarioPorId(idUsuario))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var controller = CriarController();

        var result = await controller.ObterUsuarioPorId(idUsuario);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 99 nao encontrado.", notFound.Value);
        _usuarioBusinessMock.Verify(business => business.ObterUsuarioPorId(idUsuario), Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorId_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        const int idUsuario = 3;

        _usuarioBusinessMock
            .Setup(business => business.ObterUsuarioPorId(idUsuario))
            .ThrowsAsync(new InvalidOperationException("Falha ao obter usuario"));

        var controller = CriarController();

        var result = await controller.ObterUsuarioPorId(idUsuario);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _usuarioBusinessMock.Verify(business => business.ObterUsuarioPorId(idUsuario), Times.Once);
    }

    [Fact]
    public async Task AlterarUsuario_QuandoUsuarioExiste_DeveRetornarOkComUsuario()
    {
        var request = CriarRequest(4, "usuario.alterado");
        var response = CriarResponse(4, request.Login);

        _usuarioBusinessMock
            .Setup(business => business.AlterarUsuario(request))
            .ReturnsAsync(response);

        var controller = CriarController();

        var result = await controller.AlterarUsuario(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        _usuarioBusinessMock.Verify(business => business.AlterarUsuario(request), Times.Once);
    }

    [Fact]
    public async Task AlterarUsuario_QuandoBusinessRetornaNull_DeveRetornarNotFound()
    {
        var request = CriarRequest(99, "usuario.inexistente");

        _usuarioBusinessMock
            .Setup(business => business.AlterarUsuario(request))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var controller = CriarController();

        var result = await controller.AlterarUsuario(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 99 nao encontrado.", notFound.Value);
        _usuarioBusinessMock.Verify(business => business.AlterarUsuario(request), Times.Once);
    }

    [Fact]
    public async Task AlterarUsuario_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        var request = CriarRequest(4, "usuario.erro");

        _usuarioBusinessMock
            .Setup(business => business.AlterarUsuario(request))
            .ThrowsAsync(new InvalidOperationException("Falha ao alterar usuario"));

        var controller = CriarController();

        var result = await controller.AlterarUsuario(request);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _usuarioBusinessMock.Verify(business => business.AlterarUsuario(request), Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_QuandoUsuarioExiste_DeveRetornarOkComMensagem()
    {
        const int idUsuario = 5;

        _usuarioBusinessMock
            .Setup(business => business.ApagarUsuario(idUsuario))
            .ReturnsAsync(true);

        var controller = CriarController();

        var result = await controller.ApagarUsuario(idUsuario);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Usuario com ID 5 apagado com sucesso.", ok.Value);
        _usuarioBusinessMock.Verify(business => business.ApagarUsuario(idUsuario), Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_QuandoBusinessRetornaFalse_DeveRetornarNotFound()
    {
        const int idUsuario = 99;

        _usuarioBusinessMock
            .Setup(business => business.ApagarUsuario(idUsuario))
            .ReturnsAsync(false);

        var controller = CriarController();

        var result = await controller.ApagarUsuario(idUsuario);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario com ID 99 nao encontrado.", notFound.Value);
        _usuarioBusinessMock.Verify(business => business.ApagarUsuario(idUsuario), Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        const int idUsuario = 5;

        _usuarioBusinessMock
            .Setup(business => business.ApagarUsuario(idUsuario))
            .ThrowsAsync(new InvalidOperationException("Falha ao apagar usuario"));

        var controller = CriarController();

        var result = await controller.ApagarUsuario(idUsuario);

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _usuarioBusinessMock.Verify(business => business.ApagarUsuario(idUsuario), Times.Once);
    }

    [Theory]
    [InlineData(null, "123456")]
    [InlineData("", "123456")]
    [InlineData("usuario.login", null)]
    [InlineData("usuario.login", "")]
    public async Task LoginUsuario_ComEmailOuSenhaVazios_DeveRetornarBadRequestSemLogar(
        string? email,
        string? senha)
    {
        var controller = CriarController();

        var result = await controller.LoginUsuario(email!, senha!);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("LoginUsuario() - Email e senha sao obrigatorios.", badRequest.Value);
        _usuarioBusinessMock.Verify(
            business => business.LoginUsuario(It.IsAny<LoginModelRequest>()),
            Times.Never);
        _jwtTokenServiceMock.Verify(
            service => service.PreencherTokens(It.IsAny<UsuarioModelResponse>()),
            Times.Never);
        _cacheServiceMock.Verify(
            cache => cache.Set(It.IsAny<string>(), It.IsAny<TokenModelResponse>(), It.IsAny<DateTimeOffset>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginUsuario_QuandoUsuarioNaoExiste_DeveRetornarNotFound()
    {
        const string email = "usuario.inexistente";
        const string senha = "123456";

        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.Is<LoginModelRequest>(
                model => model.Login == email && model.Senha == senha)))
            .ReturnsAsync((UsuarioModelResponse)null!);

        var controller = CriarController();

        var result = await controller.LoginUsuario(email, senha);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Usuario nao encontrado.", notFound.Value);
        _usuarioBusinessMock.Verify(
            business => business.LoginUsuario(It.Is<LoginModelRequest>(
                model => model.Login == email && model.Senha == senha)),
            Times.Once);
        _jwtTokenServiceMock.Verify(
            service => service.PreencherTokens(It.IsAny<UsuarioModelResponse>()),
            Times.Never);
        _cacheServiceMock.Verify(
            cache => cache.Set(It.IsAny<string>(), It.IsAny<TokenModelResponse>(), It.IsAny<DateTimeOffset>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginUsuario_QuandoTokenPossuiExpiracao_DeveRetornarOkEArmazenarTokenNoCache()
    {
        const string email = "usuario.login";
        const string senha = "123456";
        var usuario = CriarResponse(6, email);
        var token = CriarToken(usuario.IdUsuario, new DateTime(2026, 7, 10, 12, 0, 0));

        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.Is<LoginModelRequest>(
                model => model.Login == email && model.Senha == senha)))
            .ReturnsAsync(usuario);
        _jwtTokenServiceMock
            .Setup(service => service.PreencherTokens(usuario))
            .Returns(token);

        var controller = CriarController();

        var result = await controller.LoginUsuario(email, senha);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(usuario, ok.Value);
        _jwtTokenServiceMock.Verify(service => service.PreencherTokens(usuario), Times.Once);
        _cacheServiceMock.Verify(
            cache => cache.Set(
                "LoginUsuario:Token:6.",
                token,
                new DateTimeOffset(token.TokenExpiraEm!.Value)),
            Times.Once);
    }

    [Fact]
    public async Task LoginUsuario_QuandoTokenNaoPossuiExpiracao_DeveRetornarOkSemArmazenarNoCache()
    {
        const string email = "usuario.login";
        const string senha = "123456";
        var usuario = CriarResponse(7, email);
        var token = CriarToken(usuario.IdUsuario, null);

        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.IsAny<LoginModelRequest>()))
            .ReturnsAsync(usuario);
        _jwtTokenServiceMock
            .Setup(service => service.PreencherTokens(usuario))
            .Returns(token);

        var controller = CriarController();

        var result = await controller.LoginUsuario(email, senha);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(usuario, ok.Value);
        _jwtTokenServiceMock.Verify(service => service.PreencherTokens(usuario), Times.Once);
        _cacheServiceMock.Verify(
            cache => cache.Set(It.IsAny<string>(), It.IsAny<TokenModelResponse>(), It.IsAny<DateTimeOffset>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginUsuario_QuandoBusinessLancaExcecao_DeveRetornarStatus500()
    {
        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.IsAny<LoginModelRequest>()))
            .ThrowsAsync(new InvalidOperationException("Falha ao fazer login"));

        var controller = CriarController();

        var result = await controller.LoginUsuario("usuario.erro", "123456");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _jwtTokenServiceMock.Verify(
            service => service.PreencherTokens(It.IsAny<UsuarioModelResponse>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginUsuario_QuandoServicoTokenLancaExcecao_DeveRetornarStatus500()
    {
        var usuario = CriarResponse(8, "usuario.token");

        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.IsAny<LoginModelRequest>()))
            .ReturnsAsync(usuario);
        _jwtTokenServiceMock
            .Setup(service => service.PreencherTokens(usuario))
            .Throws(new InvalidOperationException("Falha ao preencher token"));

        var controller = CriarController();

        var result = await controller.LoginUsuario("usuario.token", "123456");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _cacheServiceMock.Verify(
            cache => cache.Set(It.IsAny<string>(), It.IsAny<TokenModelResponse>(), It.IsAny<DateTimeOffset>()),
            Times.Never);
    }

    [Fact]
    public async Task LoginUsuario_QuandoCacheLancaExcecao_DeveRetornarStatus500()
    {
        var usuario = CriarResponse(9, "usuario.cache");
        var token = CriarToken(usuario.IdUsuario, new DateTime(2026, 7, 10, 12, 0, 0));

        _usuarioBusinessMock
            .Setup(business => business.LoginUsuario(It.IsAny<LoginModelRequest>()))
            .ReturnsAsync(usuario);
        _jwtTokenServiceMock
            .Setup(service => service.PreencherTokens(usuario))
            .Returns(token);
        _cacheServiceMock
            .Setup(cache => cache.Set(
                It.IsAny<string>(),
                It.IsAny<TokenModelResponse>(),
                It.IsAny<DateTimeOffset>()))
            .Throws(new InvalidOperationException("Falha ao gravar cache"));

        var controller = CriarController();

        var result = await controller.LoginUsuario("usuario.cache", "123456");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, status.StatusCode);
        Assert.Equal("Ocorreu um erro ao processar a solicitacao.", status.Value);
        _cacheServiceMock.Verify(
            cache => cache.Set("LoginUsuario:Token:9.", token, new DateTimeOffset(token.TokenExpiraEm!.Value)),
            Times.Once);
    }

    private UsuarioController CriarController()
    {
        return new UsuarioController(
            _loggerMock.Object,
            _usuarioBusinessMock.Object,
            _jwtTokenServiceMock.Object,
            _cacheServiceMock.Object);
    }

    private static UsuarioModelRequest CriarRequest(int idUsuario, string login)
    {
        return new UsuarioModelRequest
        {
            IdUsuario = idUsuario,
            NomeUsuario = $"Usuario {idUsuario}",
            Login = login,
            Senha = "123456",
            Endereco = "Rua Teste",
            NumeroEndereco = "100",
            Bairro = "Centro",
            IdUf = 1,
            IdCidade = 2,
            NomeSocial = $"Social {idUsuario}",
            EMail = $"{login}@teste.com",
            IdSexo = 1
        };
    }

    private static UsuarioModelResponse CriarResponse(int idUsuario, string? login)
    {
        return new UsuarioModelResponse
        {
            IdUsuario = idUsuario,
            NomeUsuario = $"Usuario {idUsuario}",
            Login = login,
            Senha = "123456",
            Endereco = "Rua Teste",
            NumeroEndereco = "100",
            Bairro = "Centro",
            IdUf = 1,
            IdCidade = 2,
            NomeSocial = $"Social {idUsuario}",
            EMail = $"{login}@teste.com",
            IdSexo = 1,
            blnRetorno = true
        };
    }

    private static TokenModelResponse CriarToken(int idUsuario, DateTime? tokenExpiraEm)
    {
        return new TokenModelResponse
        {
            Token = $"token-{idUsuario}",
            IdUsuarioToken = idUsuario,
            TokenExpiraEm = tokenExpiraEm,
            RefreshToken = $"refresh-{idUsuario}",
            RefreshTokenExpiraEm = tokenExpiraEm?.AddDays(7)
        };
    }
}
