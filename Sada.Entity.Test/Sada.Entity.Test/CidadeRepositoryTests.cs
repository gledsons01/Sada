using Microsoft.EntityFrameworkCore;
using Moq;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class CidadeRepositoryTests
{
    [Fact]
    public async Task IncluirCidadeAsync_DeveGerarProximoIdNormalizarESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdCidade = 7, IdUf = 33, DescricaoCidade = "Niteroi" },
            new() { IdCidade = 3, IdUf = 35, DescricaoCidade = "Santos" });
        mock.Invocations.Clear();
        var repository = new CidadeRepository(context);

        var result = await repository.IncluirCidadeAsync(new()
        {
            IdUf = 41,
            DescricaoCidade = "  Curitiba  "
        });

        Assert.Equal(8, result.IdCidade);
        Assert.Equal(41, result.IdUf);
        Assert.Equal("Curitiba", result.DescricaoCidade);
        Assert.Equal(3, await context.Cidades.CountAsync());
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task IncluirCidadeAsync_DescricaoEmBranco_DeveSalvarNulo(string? descricao)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new CidadeRepository(context);

        var result = await repository.IncluirCidadeAsync(new()
        {
            IdUf = 35,
            DescricaoCidade = descricao
        });

        Assert.Equal(1, result.IdCidade);
        Assert.Null(result.DescricaoCidade);
    }

    [Fact]
    public async Task ListarCidadesAsync_DeveOrdenarPorId()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdCidade = 3, IdUf = 35, DescricaoCidade = "Santos" },
            new() { IdCidade = 1, IdUf = 33, DescricaoCidade = "Rio" },
            new() { IdCidade = 2, IdUf = 41, DescricaoCidade = "Curitiba" });
        var repository = new CidadeRepository(context);

        var result = await repository.ListarCidadesAsync();

        Assert.Equal([1, 2, 3], result.Select(x => x.IdCidade));
    }

    [Fact]
    public async Task ListarCidadesPorUfAsync_DeveFiltrarEOrdenarPorDescricao()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdCidade = 1, IdUf = 35, DescricaoCidade = "Santos" },
            new() { IdCidade = 2, IdUf = 33, DescricaoCidade = "Niteroi" },
            new() { IdCidade = 3, IdUf = 35, DescricaoCidade = "Campinas" });
        var repository = new CidadeRepository(context);

        var result = await repository.ListarCidadesPorUfAsync(35);

        Assert.Equal(["Campinas", "Santos"], result.Select(x => x.DescricaoCidade));
        Assert.All(result, x => Assert.Equal(35, x.IdUf));
    }

    [Theory]
    [InlineData(1, true)]
    [InlineData(999, false)]
    public async Task ObterCidadePorIdAsync_DeveRetornarConformeExistencia(int id, bool existe)
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new CidadeModel { IdCidade = 1, IdUf = 35, DescricaoCidade = "Campinas" });
        var repository = new CidadeRepository(context);

        var result = await repository.ObterCidadePorIdAsync(id);

        Assert.Equal(existe, result is not null);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoExiste_DeveAtualizarNormalizarESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context, new CidadeModel { IdCidade = 1, IdUf = 35, DescricaoCidade = "Campinas" });
        mock.Invocations.Clear();
        var repository = new CidadeRepository(context);

        var result = await repository.AlterarCidadeAsync(new()
        {
            IdCidade = 1,
            IdUf = 33,
            DescricaoCidade = "  Petropolis  "
        });

        Assert.NotNull(result);
        Assert.Equal(33, result.IdUf);
        Assert.Equal("Petropolis", result.DescricaoCidade);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AlterarCidadeAsync_QuandoNaoExiste_DeveRetornarNuloENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new CidadeRepository(context);

        var result = await repository.AlterarCidadeAsync(new() { IdCidade = 999 });

        Assert.Null(result);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoExiste_DeveRemoverRetornarTrueESalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        await SeedAsync(context,
            new() { IdCidade = 1, IdUf = 35, DescricaoCidade = "Remover" },
            new() { IdCidade = 2, IdUf = 35, DescricaoCidade = "Manter" });
        mock.Invocations.Clear();
        var repository = new CidadeRepository(context);

        var result = await repository.ApagarCidadeAsync(1);

        Assert.True(result);
        Assert.Equal(2, (await context.Cidades.SingleAsync()).IdCidade);
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApagarCidadeAsync_QuandoNaoExiste_DeveRetornarFalseENaoSalvar()
    {
        var mock = CriarContextoMock();
        await using var context = mock.Object;
        var repository = new CidadeRepository(context);

        Assert.False(await repository.ApagarCidadeAsync(999));
        mock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    private static Mock<SadaDbContext> CriarContextoMock()
    {
        var options = new DbContextOptionsBuilder<SadaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new Mock<SadaDbContext>(options) { CallBase = true };
    }

    private static async Task SeedAsync(SadaDbContext context, params CidadeModel[] cidades)
    {
        await context.Cidades.AddRangeAsync(cidades);
        await context.SaveChangesAsync();
    }
}
