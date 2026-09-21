using Microsoft.EntityFrameworkCore;
using Moq;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class SexoRepositoryTests
{
    [Fact]
    public async Task IncluirSexoAsync_ComRegistrosExistentes_DeveGerarProximoIdNormalizarESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new SexoModel { IdSexo = 2, Descricao = "Feminino", Sigla = "F" },
            new SexoModel { IdSexo = 7, Descricao = "Masculino", Sigla = "M" });
        mock.Invocations.Clear();
        var repository = new SexoRepository(context);

        var result = await repository.IncluirSexoAsync(new()
        {
            Descricao = "  Nao informado  ",
            Sigla = "  ni  "
        });

        Assert.NotNull(result);
        Assert.Equal(8, result.IdSexo);
        Assert.Equal("Nao informado", result.Descricao);
        Assert.Equal("NI", result.Sigla);
        var entity = await context.Sexos.SingleAsync(x => x.IdSexo == 8);
        Assert.Equal(result.Descricao, entity.Descricao);
        Assert.Equal(result.Sigla, entity.Sigla);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IncluirSexoAsync_SemRegistros_DeveIniciarIdEmUm()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new SexoRepository(context);

        var result = await repository.IncluirSexoAsync(new() { Descricao = "Feminino", Sigla = "F" });

        Assert.NotNull(result);
        Assert.Equal(1, result.IdSexo);
    }

    [Fact]
    public async Task IncluirSexoAsync_ComCamposNulos_DeveSalvarStringsVazias()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new SexoRepository(context);

        var result = await repository.IncluirSexoAsync(new() { Descricao = null, Sigla = null });

        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Descricao);
        Assert.Equal(string.Empty, result.Sigla);
    }

    [Fact]
    public async Task IncluirSexoAsync_QuandoSalvarFalha_DevePropagarDbUpdateException()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException("Falha simulada"));
        var repository = new SexoRepository(context);

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            repository.IncluirSexoAsync(new() { Descricao = "Masculino", Sigla = "M" }));

        Assert.Equal("Falha simulada", exception.Message);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarSexosAsync_DeveRetornarDadosMapeadosEOrdenadosPorId()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new SexoModel { IdSexo = 3, Descricao = "Nao informado", Sigla = "NI" },
            new SexoModel { IdSexo = 1, Descricao = "Feminino", Sigla = "F" },
            new SexoModel { IdSexo = 2, Descricao = "Masculino", Sigla = "M" });
        var repository = new SexoRepository(context);

        var result = await repository.ListarSexosAsync();

        Assert.Collection(result,
            x => { Assert.Equal(1, x.IdSexo); Assert.Equal("Feminino", x.Descricao); Assert.Equal("F", x.Sigla); },
            x => { Assert.Equal(2, x.IdSexo); Assert.Equal("Masculino", x.Descricao); Assert.Equal("M", x.Sigla); },
            x => { Assert.Equal(3, x.IdSexo); Assert.Equal("Nao informado", x.Descricao); Assert.Equal("NI", x.Sigla); });
    }

    [Fact]
    public async Task ListarSexosAsync_SemRegistros_DeveRetornarListaVazia()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new SexoRepository(context);

        var result = await repository.ListarSexosAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task ObterSexoPorIdAsync_QuandoExiste_DeveRetornarDadosMapeados()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new SexoModel { IdSexo = 1, Descricao = "Feminino", Sigla = "F" },
            new SexoModel { IdSexo = 2, Descricao = "Masculino", Sigla = "M" });
        var repository = new SexoRepository(context);

        var result = await repository.ObterSexoPorIdAsync(2);

        Assert.NotNull(result);
        Assert.Equal(2, result.IdSexo);
        Assert.Equal("Masculino", result.Descricao);
        Assert.Equal("M", result.Sigla);
    }

    [Fact]
    public async Task ObterSexoPorIdAsync_QuandoNaoExiste_DeveRetornarNulo()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new SexoModel { IdSexo = 1, Descricao = "Feminino", Sigla = "F" });
        var repository = new SexoRepository(context);

        Assert.Null(await repository.ObterSexoPorIdAsync(999));
    }

    [Fact]
    public async Task AlterarSexoAsync_QuandoExiste_DeveNormalizarAtualizarESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new SexoModel { IdSexo = 1, Descricao = "Original", Sigla = "OR" });
        mock.Invocations.Clear();
        var repository = new SexoRepository(context);

        var result = await repository.AlterarSexoAsync(new()
        {
            IdSexo = 1,
            Descricao = "  Alterado  ",
            Sigla = "  al  "
        });

        Assert.NotNull(result);
        Assert.Equal(1, result.IdSexo);
        Assert.Equal("Alterado", result.Descricao);
        Assert.Equal("AL", result.Sigla);
        var entity = await context.Sexos.SingleAsync();
        Assert.Equal("Alterado", entity.Descricao);
        Assert.Equal("AL", entity.Sigla);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarSexoAsync_ComCamposNulos_DeveSalvarStringsVazias()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new SexoModel { IdSexo = 1, Descricao = "Original", Sigla = "OR" });
        mock.Invocations.Clear();
        var repository = new SexoRepository(context);

        var result = await repository.AlterarSexoAsync(new SexoModelRequest { IdSexo = 1, Descricao = null, Sigla = null });

        Assert.NotNull(result);
        Assert.Equal(string.Empty, result.Descricao);
        Assert.Equal(string.Empty, result.Sigla);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarSexoAsync_QuandoNaoExiste_DeveRetornarNuloENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new SexoRepository(context);

        var result = await repository.AlterarSexoAsync(new()
        {
            IdSexo = 999,
            Descricao = "Inexistente",
            Sigla = "IN"
        });

        Assert.Null(result);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApagarSexoAsync_QuandoExiste_DeveRemoverRetornarTrueESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new SexoModel { IdSexo = 1, Descricao = "Remover", Sigla = "R" },
            new SexoModel { IdSexo = 2, Descricao = "Manter", Sigla = "M" });
        mock.Invocations.Clear();
        var repository = new SexoRepository(context);

        var result = await repository.ApagarSexoAsync(1);

        Assert.True(result);
        Assert.Equal(2, (await context.Sexos.SingleAsync()).IdSexo);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApagarSexoAsync_QuandoNaoExiste_DeveRetornarFalseENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new SexoModel { IdSexo = 1, Descricao = "Manter", Sigla = "M" });
        mock.Invocations.Clear();
        var repository = new SexoRepository(context);

        var result = await repository.ApagarSexoAsync(999);

        Assert.False(result);
        Assert.Equal(1, await context.Sexos.CountAsync());
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<SadaDbContext> CriarContextoMock()
    {
        var options = new DbContextOptionsBuilder<SadaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new Mock<SadaDbContext>(options) { CallBase = true };
    }

    private static async Task SeedAsync(SadaDbContext context, params SexoModel[] sexos)
    {
        await context.Sexos.AddRangeAsync(sexos);
        await context.SaveChangesAsync();
    }
}
