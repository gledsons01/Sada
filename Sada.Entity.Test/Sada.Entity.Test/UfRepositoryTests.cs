using Microsoft.EntityFrameworkCore;
using Moq;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class UfRepositoryTests
{
    [Fact]
    public async Task IncluirUfAsync_ComRegistrosExistentes_DeveGerarProximoIdNormalizarEPersistir()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdUf = 3, SiglaUf = "RJ", Sigla = "Rio de Janeiro" },
            new() { IdUf = 7, SiglaUf = "SP", Sigla = "Sao Paulo" });
        mock.Invocations.Clear();

        var result = await new UfRepository(context).IncluirUfAsync(new()
        {
            IdUf = 999, SiglaUf = "  pr  ", Sigla = "  Parana  "
        });

        Assert.Equal(8, result.IdUf);
        Assert.Equal("PR", result.SiglaUf);
        Assert.Equal("Parana", result.Sigla);
        Assert.Same(result, await context.Ufs.SingleAsync(x => x.IdUf == 8));
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IncluirUfAsync_SemRegistros_DeveIniciarIdEmUm()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var result = await new UfRepository(context).IncluirUfAsync(new() { SiglaUf = "SP", Sigla = "Sao Paulo" });
        Assert.Equal(1, result.IdUf);
        Assert.Equal(1, await context.Ufs.CountAsync());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task IncluirUfAsync_SiglaUfNulaOuEmBranco_DeveSalvarNulo(string? siglaUf)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var result = await new UfRepository(context).IncluirUfAsync(new() { SiglaUf = siglaUf, Sigla = "Descricao" });
        Assert.Null(result.SiglaUf);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData("  Sao Paulo  ", "Sao Paulo")]
    public async Task IncluirUfAsync_DeveApararDescricao(string? descricao, string? esperado)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var result = await new UfRepository(context).IncluirUfAsync(new() { SiglaUf = "SP", Sigla = descricao });
        Assert.Equal(esperado, result.Sigla);
    }

    [Fact]
    public async Task IncluirUfAsync_QuandoSalvarFalha_DevePropagarExcecao()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException("Falha simulada"));

        var exception = await Assert.ThrowsAsync<DbUpdateException>(() =>
            new UfRepository(context).IncluirUfAsync(new() { SiglaUf = "SP", Sigla = "Sao Paulo" }));

        Assert.Equal("Falha simulada", exception.Message);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarUfsAsync_ComRegistros_DeveRetornarTodosOrdenadosPorId()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" },
            new() { IdUf = 33, SiglaUf = "RJ", Sigla = "Rio de Janeiro" },
            new() { IdUf = 41, SiglaUf = "PR", Sigla = "Parana" });

        var result = await new UfRepository(context).ListarUfsAsync();

        Assert.Collection(result,
            x => { Assert.Equal(33, x.IdUf); Assert.Equal("RJ", x.SiglaUf); Assert.Equal("Rio de Janeiro", x.Sigla); },
            x => { Assert.Equal(35, x.IdUf); Assert.Equal("SP", x.SiglaUf); Assert.Equal("Sao Paulo", x.Sigla); },
            x => { Assert.Equal(41, x.IdUf); Assert.Equal("PR", x.SiglaUf); Assert.Equal("Parana", x.Sigla); });
    }

    [Fact]
    public async Task ListarUfsAsync_SemRegistros_DeveRetornarListaVazia()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var result = await new UfRepository(context).ListarUfsAsync();
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Theory]
    [InlineData(35, true)]
    [InlineData(999, false)]
    public async Task ObterUfPorIdAsync_DeveRetornarConformeExistencia(int idUf, bool existe)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new UfModel { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" });

        var result = await new UfRepository(context).ObterUfPorIdAsync(idUf);

        Assert.Equal(existe, result is not null);
        if (result is not null)
        {
            Assert.Equal("SP", result.SiglaUf);
            Assert.Equal("Sao Paulo", result.Sigla);
        }
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoExiste_DeveNormalizarAtualizarESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new UfModel { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" });
        mock.Invocations.Clear();

        var result = await new UfRepository(context).AlterarUfAsync(new()
        {
            IdUf = 35, SiglaUf = "  rj  ", Sigla = "  Rio de Janeiro  "
        });

        Assert.NotNull(result);
        Assert.Equal(35, result.IdUf);
        Assert.Equal("RJ", result.SiglaUf);
        Assert.Equal("Rio de Janeiro", result.Sigla);
        Assert.Same(result, await context.Ufs.SingleAsync());
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null, null, null, null)]
    [InlineData("", "", null, "")]
    [InlineData("   ", "   ", null, "")]
    public async Task AlterarUfAsync_ComCamposNulosOuEmBranco_DeveAplicarNormalizacao(
        string? siglaUf, string? descricao, string? siglaUfEsperada, string? descricaoEsperada)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new UfModel { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" });
        mock.Invocations.Clear();

        var result = await new UfRepository(context).AlterarUfAsync(new()
        {
            IdUf = 35, SiglaUf = siglaUf, Sigla = descricao
        });

        Assert.NotNull(result);
        Assert.Equal(siglaUfEsperada, result.SiglaUf);
        Assert.Equal(descricaoEsperada, result.Sigla);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarUfAsync_QuandoNaoExiste_DeveRetornarNuloENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var result = await new UfRepository(context).AlterarUfAsync(new() { IdUf = 999, SiglaUf = "SP", Sigla = "Sao Paulo" });
        Assert.Null(result);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApagarUfAsync_QuandoExiste_DeveRemoverRetornarTrueESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdUf = 33, SiglaUf = "RJ", Sigla = "Rio de Janeiro" },
            new() { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" });
        mock.Invocations.Clear();

        var result = await new UfRepository(context).ApagarUfAsync(33);

        Assert.True(result);
        Assert.Equal(35, (await context.Ufs.SingleAsync()).IdUf);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApagarUfAsync_QuandoNaoExiste_DeveRetornarFalseENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new UfModel { IdUf = 35, SiglaUf = "SP", Sigla = "Sao Paulo" });
        mock.Invocations.Clear();

        var result = await new UfRepository(context).ApagarUfAsync(999);

        Assert.False(result);
        Assert.Equal(1, await context.Ufs.CountAsync());
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ListarUfsAsync_ComTokenCancelado_DevePropagarCancelamento()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            new UfRepository(context).ListarUfsAsync(cancellation.Token));
    }

    private static Mock<SadaDbContext> CriarContextoMock()
    {
        var options = new DbContextOptionsBuilder<SadaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new Mock<SadaDbContext>(options) { CallBase = true };
    }

    private static async Task SeedAsync(SadaDbContext context, params UfModel[] ufs)
    {
        await context.Ufs.AddRangeAsync(ufs);
        await context.SaveChangesAsync();
    }
}
