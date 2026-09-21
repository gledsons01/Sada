using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class UsuarioRepositoryTests
{
    [Fact]
    public async Task Incluir_SemRegistros_GeraIdUmEMapeiaCampos()
    {
        await using var db = NovoDb();
        var result = await Repo(db).IncluirUsuarioAsync(Request());
        Assert.Equal(1, result.IdUsuario);
        Assert.True(result.blnRetorno);
        AssertEditado(await db.Usuarios.SingleAsync(), 1, 10, 20);
    }

    [Fact]
    public async Task Incluir_ComRegistros_GeraMaiorIdMaisUm()
    {
        await using var db = NovoDb();
        db.AddRange(Usuario(2), Usuario(7));
        await db.SaveChangesAsync();
        var result = await Repo(db).IncluirUsuarioAsync(Request());
        Assert.Equal(8, result.IdUsuario);
        Assert.True(await db.Usuarios.AnyAsync(x => x.IdUsuario == 8));
    }

    [Fact]
    public async Task Incluir_ComUfECidadeNulos_PersisteZero()
    {
        await using var db = NovoDb();
        var request = Request();
        request.IdUf = null;
        request.IdCidade = null;
        await Repo(db).IncluirUsuarioAsync(request);
        var entity = await db.Usuarios.SingleAsync();
        Assert.Equal(0, entity.IdUf);
        Assert.Equal(0, entity.IdCidade);
    }

    [Fact]
    public async Task Listar_SemRegistros_RetornaListaVazia()
    {
        await using var db = NovoDb();
        Assert.Empty(await Repo(db).ListarUsuariosAsync());
    }

    [Fact]
    public async Task Listar_OrdenaPorIdEMapeiaTodosCampos()
    {
        await using var db = NovoDb();
        db.AddRange(Usuario(3), Usuario(1));
        await db.SaveChangesAsync();
        var result = await Repo(db).ListarUsuariosAsync();
        Assert.Equal(new[] { 1, 3 }, result.Select(x => x.IdUsuario));
        AssertResponse(result[0], 1);
    }

    [Fact]
    public async Task Listar_QuandoContextoFalha_LogaEPropagaExcecao()
    {
        var db = NovoDb();
        var logger = new Mock<ILogger<UsuarioRepository>>();
        var repo = new UsuarioRepository(db, logger.Object);
        await db.DisposeAsync();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => repo.ListarUsuariosAsync());
        logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
            It.Is<It.IsAnyType>((state, _) => state.ToString() == "Erro ao listar usuarios."),
            It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [Fact]
    public async Task Alterar_UsuarioExistente_AtualizaTodosCampos()
    {
        await using var db = NovoDb();
        db.Add(Usuario(4));
        await db.SaveChangesAsync();
        var request = Request();
        request.IdUsuario = 4;
        var result = await Repo(db).AlterarUsuarioAsync(request);
        Assert.NotNull(result);
        Assert.Equal(4, result.IdUsuario);
        Assert.True(result.blnRetorno);
        AssertEditado(await db.Usuarios.SingleAsync(), 4, 10, 20);
    }

    [Fact]
    public async Task Alterar_ComUfECidadeNulos_AtualizaComZero()
    {
        await using var db = NovoDb();
        db.Add(Usuario(4));
        await db.SaveChangesAsync();
        var request = Request();
        request.IdUsuario = 4;
        request.IdUf = null;
        request.IdCidade = null;
        await Repo(db).AlterarUsuarioAsync(request);
        var entity = await db.Usuarios.SingleAsync();
        Assert.Equal(0, entity.IdUf);
        Assert.Equal(0, entity.IdCidade);
    }

    [Fact]
    public async Task Alterar_UsuarioInexistente_RetornaNulo()
    {
        await using var db = NovoDb();
        Assert.Null(await Repo(db).AlterarUsuarioAsync(Request()));
        Assert.Empty(db.Usuarios);
    }

    [Fact]
    public async Task Apagar_UsuarioExistente_ExcluiERetornaVerdadeiro()
    {
        await using var db = NovoDb();
        db.Add(Usuario(5));
        await db.SaveChangesAsync();
        Assert.True(await Repo(db).ApagarUsuarioAsync(5));
        Assert.Empty(await db.Usuarios.ToListAsync());
    }

    [Fact]
    public async Task Apagar_UsuarioInexistente_RetornaFalso()
    {
        await using var db = NovoDb();
        Assert.False(await Repo(db).ApagarUsuarioAsync(999));
    }

    [Fact]
    public async Task ObterPorId_UsuarioExistente_MapeiaTodosCampos()
    {
        await using var db = NovoDb();
        db.Add(Usuario(6));
        await db.SaveChangesAsync();
        var result = await Repo(db).ObterUsuarioPorIdAsync(6);
        Assert.NotNull(result);
        AssertResponse(result, 6);
    }

    [Fact]
    public async Task ObterPorId_UsuarioInexistente_RetornaNulo()
    {
        await using var db = NovoDb();
        Assert.Null(await Repo(db).ObterUsuarioPorIdAsync(999));
    }

    [Fact]
    public async Task Login_CredenciaisValidas_RemoveEspacosEMapeiaResposta()
    {
        await using var db = NovoDb();
        db.Add(Usuario(9));
        await db.SaveChangesAsync();
        var result = await Repo(db).LoginUsuarioAsync(new LoginModelRequest
        {
            Login = "  login9  ",
            Senha = "  senha9  "
        });
        Assert.Equal(9, result.IdUsuario);
        Assert.Equal("Nome 9", result.NomeUsuario);
        Assert.Equal("login9", result.Login);
        Assert.Equal("usuario9@teste.com", result.EMail);
        Assert.True(result.blnRetorno);
    }

    [Theory]
    [InlineData("login9", "incorreta")]
    [InlineData("inexistente", "senha9")]
    public async Task Login_CredenciaisInvalidas_LancaExcecao(string login, string senha)
    {
        await using var db = NovoDb();
        db.Add(Usuario(9));
        await db.SaveChangesAsync();
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            Repo(db).LoginUsuarioAsync(new LoginModelRequest { Login = login, Senha = senha }));
        Assert.Equal("Usuario ou senha invalidos", error.Message);
    }

    [Fact]
    public async Task Listar_TokenCancelado_PropagaCancelamento()
    {
        await using var db = NovoDb();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Repo(db).ListarUsuariosAsync(cancellation.Token));
    }

    private static SadaDbContext NovoDb() => new(new DbContextOptionsBuilder<SadaDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static UsuarioRepository Repo(SadaDbContext db) =>
        new(db, Mock.Of<ILogger<UsuarioRepository>>());

    private static UsuarioModelRequest Request() => new()
    {
        IdUsuario = 999, NomeUsuario = "  Novo Nome  ", Login = "  novo.login  ",
        Senha = "  segredo  ", Endereco = "  Nova Rua  ", NumeroEndereco = "  100  ",
        Bairro = "  Novo Bairro  ", IdUf = 10, IdCidade = 20,
        NomeSocial = "  Novo Social  ", IdSexo = 2, EMail = "  novo@teste.com  "
    };

    private static UsuarioModel Usuario(int id) => new()
    {
        IdUsuario = id, NomeUsuario = $"Nome {id}", Login = $"login{id}", Senha = $"senha{id}",
        Endereco = $"Rua {id}", NumeroEndereco = id.ToString(), Bairro = $"Bairro {id}",
        IdUf = 10, IdCidade = 20, NomeSocial = $"Social {id}", ID_SEXO = 2,
        E_MAIL = $"usuario{id}@teste.com"
    };

    private static void AssertEditado(UsuarioModel x, int id, int uf, int cidade)
    {
        Assert.Equal(id, x.IdUsuario);
        Assert.Equal("Novo Nome", x.NomeUsuario);
        Assert.Equal("novo.login", x.Login);
        Assert.Equal("segredo", x.Senha);
        Assert.Equal("Nova Rua", x.Endereco);
        Assert.Equal("100", x.NumeroEndereco);
        Assert.Equal("Novo Bairro", x.Bairro);
        Assert.Equal(uf, x.IdUf);
        Assert.Equal(cidade, x.IdCidade);
        Assert.Equal("Novo Social", x.NomeSocial);
        Assert.Equal(2, x.ID_SEXO);
        Assert.Equal("novo@teste.com", x.E_MAIL);
    }

    private static void AssertResponse(Sada.Api.Entity.Model.Response.UsuarioModelResponse x, int id)
    {
        Assert.Equal(id, x.IdUsuario);
        Assert.Equal($"Nome {id}", x.NomeUsuario);
        Assert.Equal($"login{id}", x.Login);
        Assert.Equal($"senha{id}", x.Senha);
        Assert.Equal($"Rua {id}", x.Endereco);
        Assert.Equal(id.ToString(), x.NumeroEndereco);
        Assert.Equal($"Bairro {id}", x.Bairro);
        Assert.Equal(10, x.IdUf);
        Assert.Equal(20, x.IdCidade);
        Assert.Equal($"Social {id}", x.NomeSocial);
        Assert.Equal(2, x.IdSexo);
        Assert.Equal($"usuario{id}@teste.com", x.EMail);
        Assert.True(x.blnRetorno);
    }
}
