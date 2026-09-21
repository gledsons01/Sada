using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using UsuarioBusiness = Sada.Api.Business.Usuario;

namespace Sada.Api.Business.Test;

public sealed class UsuarioTests
{
    [Fact]
    public void Usuario_DeveImplementarInterfaceIUsuario()
    {
        var (service, _, _) = CriarServico();

        Assert.IsAssignableFrom<IUsuario>(service);
    }

    [Fact]
    public async Task ListUsuariosAsync_QuandoExistemRegistros_DeveRetornarMesmaListaELogarQuantidade()
    {
        var expected = new List<UsuarioModelResponse> { CriarResponse(1), CriarResponse(2) };
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarUsuariosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.ListUsuariosAsync();

        Assert.Same(expected, result);
        repository.Verify(x => x.ListarUsuariosAsync(It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Listagem de Registros Cadastrados -  2.");
    }

    [Fact]
    public async Task ListUsuariosAsync_QuandoNaoExistemRegistros_DeveRetornarListaVaziaELogarZero()
    {
        var expected = new List<UsuarioModelResponse>();
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarUsuariosAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.ListUsuariosAsync();

        Assert.Same(expected, result);
        Assert.Empty(result);
        VerificarLog(logger, LogLevel.Information, "Listagem de Registros Cadastrados -  0.");
    }

    [Fact]
    public async Task ListUsuariosAsync_QuandoRepositorioFalha_DevePropagarExcecao()
    {
        var expected = new InvalidOperationException("Falha ao listar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ListarUsuariosAsync(It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ListUsuariosAsync());

        Assert.Same(expected, exception);
        VerificarAusenciaDeLogs(logger);
    }

    [Fact]
    public async Task CadastrarUsuarioAsync_DeveRepassarRequestRetornarRespostaELogarSucesso()
    {
        var request = CriarRequest();
        var expected = CriarResponse(10);
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.CadastrarUsuarioAsync(request);

        Assert.Same(expected, result);
        repository.Verify(x => x.IncluirUsuarioAsync(
            It.Is<UsuarioModelRequest>(x => ReferenceEquals(x, request)),
            It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Cadastro de Usuario Nome Teste efetuado com sucesso.");
    }

    [Fact]
    public async Task CadastrarUsuarioAsync_QuandoRepositorioFalha_DeveLogarErroERelancar()
    {
        var request = CriarRequest();
        var expected = new InvalidOperationException("Falha ao cadastrar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CadastrarUsuarioAsync(request));

        Assert.Same(expected, exception);
        VerificarLog(logger, LogLevel.Error, "Erro ao cadastrar usuario Nome Teste", expected);
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoExiste_DeveRetornarRespostaELogarSucesso()
    {
        var request = CriarRequest(7);
        var expected = CriarResponse(7);
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.AlterarUsuarioAsync(request);

        Assert.Same(expected, result);
        repository.Verify(x => x.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Alteração de Usuario Nome Teste efetuada com sucesso.");
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoNaoExiste_DeveLogarAvisoELancarKeyNotFoundException()
    {
        var request = CriarRequest(999);
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AlterarUsuarioAsync(request));

        Assert.Equal("Usuario com ID 999 não encontrado.", exception.Message);
        VerificarLog(logger, LogLevel.Warning, "Usuario não encontrado para alteração. ID: 999");
    }

    [Fact]
    public async Task AlterarUsuarioAsync_QuandoRepositorioFalha_DevePropagarExcecaoSemLogar()
    {
        var request = CriarRequest(7);
        var expected = new InvalidOperationException("Falha ao alterar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AlterarUsuarioAsync(request));

        Assert.Same(expected, exception);
        VerificarAusenciaDeLogs(logger);
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoExiste_DeveRetornarTrueELogarSucesso()
    {
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarUsuarioAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await service.ApagarUsuarioAsync(7);

        Assert.True(result);
        repository.Verify(x => x.ApagarUsuarioAsync(7, It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Exclusão de Usuario ID 7 efetuada com sucesso.");
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoNaoExiste_DeveLogarAvisoELancarKeyNotFoundException()
    {
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarUsuarioAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ApagarUsuarioAsync(999));

        Assert.Equal("Usuario com ID 999 não encontrado.", exception.Message);
        VerificarLog(logger, LogLevel.Warning, "Usuario não encontrado para exclusão. ID: 999");
    }

    [Fact]
    public async Task ApagarUsuarioAsync_QuandoRepositorioFalha_DevePropagarExcecaoSemLogar()
    {
        var expected = new InvalidOperationException("Falha ao apagar");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ApagarUsuarioAsync(7, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApagarUsuarioAsync(7));

        Assert.Same(expected, exception);
        VerificarAusenciaDeLogs(logger);
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoExiste_DeveRetornarRespostaCompletaELogarSucesso()
    {
        var expected = CriarResponse(7);
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterUsuarioPorIdAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.ObterUsuarioPorIdAsync(7);

        Assert.Same(expected, result);
        AssertResponseCompleta(result!, 7);
        repository.Verify(x => x.ObterUsuarioPorIdAsync(7, It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Consulta de Usuario ID 7 efetuada com sucesso.");
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoNaoExiste_DeveLogarAvisoELancarKeyNotFoundException()
    {
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterUsuarioPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ObterUsuarioPorIdAsync(999));

        Assert.Equal("Usuario com ID 999 não encontrado.", exception.Message);
        VerificarLog(logger, LogLevel.Warning, "Usuario não encontrado. ID: 999");
    }

    [Fact]
    public async Task ObterUsuarioPorIdAsync_QuandoRepositorioFalha_DevePropagarExcecaoSemLogar()
    {
        var expected = new InvalidOperationException("Falha ao obter");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.ObterUsuarioPorIdAsync(7, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ObterUsuarioPorIdAsync(7));

        Assert.Same(expected, exception);
        VerificarAusenciaDeLogs(logger);
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoCredenciaisValidas_DeveRetornarRespostaELogarSucesso()
    {
        var request = new LoginModelRequest { Login = "usuario", Senha = "senha" };
        var expected = CriarResponse(7);
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.LoginUsuarioAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await service.LoginUsuarioAsync(request);

        Assert.Same(expected, result);
        repository.Verify(x => x.LoginUsuarioAsync(
            It.Is<LoginModelRequest>(x => ReferenceEquals(x, request)),
            It.IsAny<CancellationToken>()), Times.Once);
        VerificarLog(logger, LogLevel.Information, "Login bem-sucedido para o usuário: usuario");
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoRepositorioRetornaNulo_DeveLogarAvisoELancarUnauthorizedAccessException()
    {
        var request = new LoginModelRequest { Login = "inexistente", Senha = "senha" };
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.LoginUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse)null!);

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginUsuarioAsync(request));

        Assert.Equal("Login falhou para o usuário: inexistente", exception.Message);
        VerificarLog(logger, LogLevel.Warning, "Login falhou para o usuário: inexistente");
    }

    [Fact]
    public async Task LoginUsuarioAsync_QuandoRepositorioFalha_DevePropagarExcecaoSemLogar()
    {
        var request = new LoginModelRequest { Login = "usuario", Senha = "senha" };
        var expected = new InvalidOperationException("Falha no login");
        var (service, repository, logger) = CriarServico();
        repository.Setup(x => x.LoginUsuarioAsync(request, It.IsAny<CancellationToken>())).ThrowsAsync(expected);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => service.LoginUsuarioAsync(request));

        Assert.Same(expected, exception);
        VerificarAusenciaDeLogs(logger);
    }

    private static (
        UsuarioBusiness Service,
        Mock<IUsuarioRepository> Repository,
        Mock<ILogger<UsuarioBusiness>> Logger) CriarServico()
    {
        var repository = new Mock<IUsuarioRepository>(MockBehavior.Strict);
        var logger = new Mock<ILogger<UsuarioBusiness>>();
        return (new UsuarioBusiness(logger.Object, repository.Object), repository, logger);
    }

    private static UsuarioModelRequest CriarRequest(int? idUsuario = null) => new()
    {
        IdUsuario = idUsuario, NomeUsuario = "Nome Teste", Login = "usuario", Senha = "senha",
        Endereco = "Rua Teste", NumeroEndereco = "100", Bairro = "Centro",
        IdUf = 35, IdCidade = 1, NomeSocial = "Nome Social", IdSexo = 2, EMail = "teste@email.com"
    };

    private static UsuarioModelResponse CriarResponse(int id) => new()
    {
        IdUsuario = id, blnRetorno = true, NomeUsuario = "Nome Teste", Login = "usuario",
        Senha = "senha", Endereco = "Rua Teste", NumeroEndereco = "100", Bairro = "Centro",
        IdUf = 35, IdCidade = 1, NomeSocial = "Nome Social", IdSexo = 2,
        EMail = "teste@email.com", blnRetornp = true, Token = "token",
        TokenExpiraEm = new DateTime(2026, 9, 20, 12, 0, 0),
        RefreshToken = "refresh", RefreshTokenExpiraEm = new DateTime(2026, 9, 27, 12, 0, 0)
    };

    private static void AssertResponseCompleta(UsuarioModelResponse x, int id)
    {
        Assert.Equal(id, x.IdUsuario);
        Assert.True(x.blnRetorno);
        Assert.Equal("Nome Teste", x.NomeUsuario);
        Assert.Equal("usuario", x.Login);
        Assert.Equal("senha", x.Senha);
        Assert.Equal("Rua Teste", x.Endereco);
        Assert.Equal("100", x.NumeroEndereco);
        Assert.Equal("Centro", x.Bairro);
        Assert.Equal(35, x.IdUf);
        Assert.Equal(1, x.IdCidade);
        Assert.Equal("Nome Social", x.NomeSocial);
        Assert.Equal(2, x.IdSexo);
        Assert.Equal("teste@email.com", x.EMail);
        Assert.True(x.blnRetornp);
        Assert.Equal("token", x.Token);
        Assert.Equal(new DateTime(2026, 9, 20, 12, 0, 0), x.TokenExpiraEm);
        Assert.Equal("refresh", x.RefreshToken);
        Assert.Equal(new DateTime(2026, 9, 27, 12, 0, 0), x.RefreshTokenExpiraEm);
    }

    private static void VerificarLog(
        Mock<ILogger<UsuarioBusiness>> logger, LogLevel level, string mensagem, Exception? exception = null)
    {
        logger.Verify(x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((state, _) =>
                    state.ToString() != null && state.ToString()!.Contains(mensagem)),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static void VerificarAusenciaDeLogs(Mock<ILogger<UsuarioBusiness>> logger)
    {
        logger.Verify(x => x.Log(
                It.IsAny<LogLevel>(),
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
    }
}
