using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Test.Repository;

public class UsuarioTests
{
    private readonly Mock<ILogger<Usuario>> _loggerMock = new();
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();

    [Fact]
    public void Usuario_DeveImplementarInterfaceIUsuario()
    {
        var service = CriarServico();

        Assert.IsAssignableFrom<IUsuario>(service);
    }

    [Fact]
    public async Task ListUsuarios_DeveRetornarUsuariosDoRepositorio()
    {
        var usuarios = new List<UsuarioModelResponse>
        {
            CriarResponse(1, "usuario1"),
            CriarResponse(2, "usuario2")
        };

        _usuarioRepositoryMock
            .Setup(repository => repository.ListarUsuariosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarios);

        var service = CriarServico();

        var result = await service.ListUsuarios();

        Assert.Same(usuarios, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.ListarUsuariosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListUsuarios_QuandoRepositorioRetornaListaVazia_DeveRetornarListaVazia()
    {
        var usuarios = new List<UsuarioModelResponse>();

        _usuarioRepositoryMock
            .Setup(repository => repository.ListarUsuariosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuarios);

        var service = CriarServico();

        var result = await service.ListUsuarios();

        Assert.Empty(result);
        _usuarioRepositoryMock.Verify(
            repository => repository.ListarUsuariosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CadastrarUsuario_QuandoRepositorioRetornaSucesso_DeveRetornarResposta()
    {
        var request = CriarRequest(0, "novo.usuario");
        var response = CriarResponse(10, request.Login);

        _usuarioRepositoryMock
            .Setup(repository => repository.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.CadastrarUsuario(request);

        Assert.Same(response, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CadastrarUsuario_QuandoRepositorioLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarRequest(0, "usuario.erro");
        var exception = new InvalidOperationException("Falha ao cadastrar usuario");

        _usuarioRepositoryMock
            .Setup(repository => repository.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var service = CriarServico();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CadastrarUsuario(request));

        Assert.Same(exception, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.IncluirUsuarioAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AlterarUsuario_QuandoUsuarioExiste_DeveRetornarResposta()
    {
        var request = CriarRequest(3, "usuario.alterado");
        var response = CriarResponse(3, request.Login);

        _usuarioRepositoryMock
            .Setup(repository => repository.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.AlterarUsuario(request);

        Assert.Same(response, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AlterarUsuario_QuandoUsuarioNaoExiste_DeveLancarKeyNotFoundException()
    {
        var request = CriarRequest(99, "usuario.inexistente");

        _usuarioRepositoryMock
            .Setup(repository => repository.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var service = CriarServico();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AlterarUsuario(request));

        Assert.Equal("Usuario com ID 99 não encontrado.", exception.Message);
        _usuarioRepositoryMock.Verify(
            repository => repository.AlterarUsuarioAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_QuandoUsuarioExiste_DeveRetornarTrue()
    {
        const int idUsuario = 4;

        _usuarioRepositoryMock
            .Setup(repository => repository.ApagarUsuarioAsync(idUsuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = CriarServico();

        var result = await service.ApagarUsuario(idUsuario);

        Assert.True(result);
        _usuarioRepositoryMock.Verify(
            repository => repository.ApagarUsuarioAsync(idUsuario, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_QuandoUsuarioNaoExiste_DeveLancarKeyNotFoundException()
    {
        const int idUsuario = 99;

        _usuarioRepositoryMock
            .Setup(repository => repository.ApagarUsuarioAsync(idUsuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var service = CriarServico();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ApagarUsuario(idUsuario));

        Assert.Equal("Usuario com ID 99 não encontrado.", exception.Message);
        _usuarioRepositoryMock.Verify(
            repository => repository.ApagarUsuarioAsync(idUsuario, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorId_QuandoUsuarioExiste_DeveRetornarResposta()
    {
        const int idUsuario = 5;
        var response = CriarResponse(idUsuario, "usuario.consulta");

        _usuarioRepositoryMock
            .Setup(repository => repository.ObterUsuarioPorIdAsync(idUsuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.ObterUsuarioPorId(idUsuario);

        Assert.Same(response, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.ObterUsuarioPorIdAsync(idUsuario, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ObterUsuarioPorId_QuandoUsuarioNaoExiste_DeveLancarKeyNotFoundException()
    {
        const int idUsuario = 99;

        _usuarioRepositoryMock
            .Setup(repository => repository.ObterUsuarioPorIdAsync(idUsuario, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse?)null);

        var service = CriarServico();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.ObterUsuarioPorId(idUsuario));

        Assert.Equal("Usuario com ID 99 não encontrado.", exception.Message);
        _usuarioRepositoryMock.Verify(
            repository => repository.ObterUsuarioPorIdAsync(idUsuario, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginUsuario_QuandoCredenciaisValidas_DeveRetornarResposta()
    {
        var request = new LoginModelRequest
        {
            Login = "usuario.login",
            Senha = "123456"
        };
        var response = CriarResponse(6, request.Login);

        _usuarioRepositoryMock
            .Setup(repository => repository.LoginUsuario(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.LoginUsuario(request);

        Assert.Same(response, result);
        _usuarioRepositoryMock.Verify(
            repository => repository.LoginUsuario(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task LoginUsuario_QuandoRepositorioRetornaNull_DeveLancarUnauthorizedAccessException()
    {
        var request = new LoginModelRequest
        {
            Login = "usuario.invalido",
            Senha = "senha-invalida"
        };

        _usuarioRepositoryMock
            .Setup(repository => repository.LoginUsuario(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UsuarioModelResponse)null!);

        var service = CriarServico();

        var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginUsuario(request));

        Assert.Equal("Login falhou para o usuário: usuario.invalido", exception.Message);
        _usuarioRepositoryMock.Verify(
            repository => repository.LoginUsuario(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private Usuario CriarServico()
    {
        return new Usuario(_loggerMock.Object, _usuarioRepositoryMock.Object);
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
}
