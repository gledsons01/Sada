using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Api.Entity.Repository;

namespace Sada.Entity.Test;

public sealed class TituloRepositoryTests
{
    [Fact]
    public async Task IncluirTituloAsync_DeveCadastrarTituloNormalizandoCampos()
    {
        await using var context = CriarContexto();
        context.Titulos.Add(new TituloModel
        {
            IdTitulo = 7,
            Titulo = "Existente",
            Descricao = "Descricao existente",
            Vencimento = new DateTime(2026, 5, 30),
            Status = 'P'
        });
        await context.SaveChangesAsync();

        var repository = new TituloRepository(context);
        var request = new TituloModelRequest
        {
            Titulo = "  Novo titulo  ",
            Descricao = "  Nova descricao  ",
            Vencimento = new DateTime(2026, 6, 1, 15, 45, 0),
            Status = " a "
        };

        var response = await repository.IncluirTituloAsync(request);

        Assert.True(response.Retorno);
        Assert.Equal(8, response.IdTitulo);
        Assert.Equal("Novo titulo", response.Titulo);
        Assert.Equal("Nova descricao", response.Descricao);
        Assert.Equal(new DateTime(2026, 6, 1, 15, 45, 0), response.Vencimento);
        Assert.Equal('A', response.Status);

        var entity = await context.Titulos.SingleAsync(item => item.IdTitulo == 8);
        Assert.Equal(response.Titulo, entity.Titulo);
        Assert.Equal(response.Descricao, entity.Descricao);
        Assert.Equal(response.Vencimento, entity.Vencimento);
        Assert.Equal(response.Status, entity.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task IncluirTituloAsync_DevePermitirStatusNuloQuandoStatusNaoInformado(string? status)
    {
        await using var context = CriarContexto();
        var repository = new TituloRepository(context);
        var request = new TituloModelRequest
        {
            Titulo = "Titulo",
            Descricao = "Descricao",
            Status = status
        };

        var response = await repository.IncluirTituloAsync(request);

        Assert.True(response.Retorno);
        Assert.Equal(1, response.IdTitulo);
        Assert.Null(response.Status);
        Assert.Null((await context.Titulos.SingleAsync()).Status);
    }

    [Fact]
    public async Task ListarTitulosAsync_DeveRetornarTitulosOrdenadosPorId()
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel { IdTitulo = 3, Titulo = "C", Descricao = "Descricao C", Status = 'A' },
            new TituloModel { IdTitulo = 1, Titulo = "A", Descricao = "Descricao A", Status = 'P' },
            new TituloModel { IdTitulo = 2, Titulo = "B", Descricao = "Descricao B", Status = null });
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync();

        Assert.Collection(result,
            item =>
            {
                Assert.Equal(1, item.IdTitulo);
                Assert.Equal("A", item.Titulo);
                Assert.True(item.Retorno);
            },
            item =>
            {
                Assert.Equal(2, item.IdTitulo);
                Assert.Equal("B", item.Titulo);
                Assert.True(item.Retorno);
            },
            item =>
            {
                Assert.Equal(3, item.IdTitulo);
                Assert.Equal("C", item.Titulo);
                Assert.True(item.Retorno);
            });
    }

    [Fact]
    public async Task ListarTitulosAsync_QuandoNaoExistemTitulos_DeveRetornarListaVazia()
    {
        await using var context = CriarContexto();
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync();

        Assert.Empty(result);
    }

    [Fact]
    public async Task ObterTituloPorIdAsync_QuandoExiste_DeveMapearTodosOsCampos()
    {
        await using var context = CriarContexto();
        var vencimento = new DateTime(2026, 9, 30, 14, 45, 0);
        await SeedAsync(context,
            new TituloModel
            {
                IdTitulo = 1,
                Titulo = "Primeiro",
                Descricao = "Descricao primeiro",
                Vencimento = new DateTime(2026, 9, 1),
                Status = 'P'
            },
            new TituloModel
            {
                IdTitulo = 7,
                Titulo = "Titulo procurado",
                Descricao = "Descricao procurada",
                Vencimento = vencimento,
                Status = 'A'
            });
        var repository = new TituloRepository(context);

        var result = await repository.ObterTituloPorIdAsync(7);

        Assert.NotNull(result);
        Assert.Equal(7, result.IdTitulo);
        Assert.Equal("Titulo procurado", result.Titulo);
        Assert.Equal("Descricao procurada", result.Descricao);
        Assert.Equal(vencimento, result.Vencimento);
        Assert.Equal('A', result.Status);
        Assert.True(result.Retorno);
    }

    [Fact]
    public async Task ObterTituloPorIdAsync_QuandoCamposOpcionaisSaoNulos_DevePreservarValoresNulos()
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel
        {
            IdTitulo = 1,
            Titulo = "Titulo",
            Descricao = "Descricao",
            Vencimento = null,
            Status = null
        });
        var repository = new TituloRepository(context);

        var result = await repository.ObterTituloPorIdAsync(1);

        Assert.NotNull(result);
        Assert.Null(result.Vencimento);
        Assert.Null(result.Status);
        Assert.True(result.Retorno);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task ObterTituloPorIdAsync_QuandoNaoExiste_DeveRetornarNulo(int id)
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel
        {
            IdTitulo = 1,
            Titulo = "Existente",
            Descricao = "Descricao existente",
            Status = 'P'
        });
        var repository = new TituloRepository(context);

        var result = await repository.ObterTituloPorIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task ObterTituloPorIdAsync_DeveRetornarNovoResponseSemAlterarEntidadeRastreada()
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel
        {
            IdTitulo = 1,
            Titulo = "Original",
            Descricao = "Descricao original",
            Status = 'P'
        });
        var repository = new TituloRepository(context);

        var result = await repository.ObterTituloPorIdAsync(1);
        Assert.NotNull(result);
        result.Titulo = "Alterado apenas no response";

        var entity = await context.Titulos.SingleAsync();
        Assert.Equal("Original", entity.Titulo);
    }

    [Fact]
    public async Task ObterTituloPorIdAsync_ComTokenCancelado_DevePropagarCancelamento()
    {
        await using var context = CriarContexto();
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();
        var repository = new TituloRepository(context);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            repository.ObterTituloPorIdAsync(1, cancellation.Token));
    }

    [Theory]
    [InlineData("a")]
    [InlineData(" A ")]
    [InlineData("ativo")]
    public async Task ListarTitulosAsync_ComStatus_DeveFiltrarPorPrimeiraLetraNormalizada(string status)
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel { IdTitulo = 1, Titulo = "A", Descricao = "Descricao A", Status = 'A' },
            new TituloModel { IdTitulo = 2, Titulo = "P", Descricao = "Descricao P", Status = 'P' },
            new TituloModel { IdTitulo = 3, Titulo = "Sem status", Descricao = "Descricao sem status", Status = null });
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync(status, null);

        var item = Assert.Single(result);
        Assert.Equal(1, item.IdTitulo);
        Assert.Equal('A', item.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ListarTitulosAsync_SemStatus_DeveIgnorarFiltroDeStatus(string? status)
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel { IdTitulo = 1, Titulo = "A", Descricao = "Descricao A", Status = 'A' },
            new TituloModel { IdTitulo = 2, Titulo = "P", Descricao = "Descricao P", Status = 'P' });
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync(status, null);
        Assert.Equal([1, 2], result.Select(item => item.IdTitulo));
    }

    [Fact]
    public async Task ListarTitulosAsync_ComVencimento_DeveCompararApenasData()
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel
            {
                IdTitulo = 1,
                Titulo = "Mesmo dia",
                Descricao = "Descricao mesmo dia",
                Vencimento = new DateTime(2026, 6, 1, 8, 0, 0),
                Status = 'A'
            },
            new TituloModel
            {
                IdTitulo = 2,
                Titulo = "Outro dia",
                Descricao = "Descricao outro dia",
                Vencimento = new DateTime(2026, 6, 2),
                Status = 'A'
            },
            new TituloModel
            {
                IdTitulo = 3,
                Titulo = "Sem vencimento",
                Descricao = "Descricao sem vencimento",
                Vencimento = null,
                Status = 'A'
            });
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync(null, new DateTime(2026, 6, 1, 23, 59, 59));

        var item = Assert.Single(result);
        Assert.Equal(1, item.IdTitulo);
    }

    [Fact]
    public async Task ListarTitulosAsync_ComStatusEVencimento_DeveAplicarTodosOsFiltros()
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel { IdTitulo = 1, Titulo = "A dia", Descricao = "Descricao A dia", Vencimento = new DateTime(2026, 6, 1), Status = 'A' },
            new TituloModel { IdTitulo = 2, Titulo = "P dia", Descricao = "Descricao P dia", Vencimento = new DateTime(2026, 6, 1), Status = 'P' },
            new TituloModel { IdTitulo = 3, Titulo = "A outro", Descricao = "Descricao A outro", Vencimento = new DateTime(2026, 6, 2), Status = 'A' });
        var repository = new TituloRepository(context);

        var result = await repository.ListarTitulosAsync("a", new DateTime(2026, 6, 1));

        var item = Assert.Single(result);
        Assert.Equal(1, item.IdTitulo);
    }

    [Fact]
    public async Task AlterarTituloAsync_QuandoTituloExiste_DeveAtualizarVencimentoEStatus()
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel
        {
            IdTitulo = 1,
            Titulo = "Titulo original",
            Descricao = "Descricao original",
            Vencimento = new DateTime(2026, 6, 1),
            Status = 'A'
        });
        var repository = new TituloRepository(context);

        var response = await repository.AlterarTituloAsync(new TituloModelEditExclusao
        {
            IdTitulo = 1,
            Vencimento = new DateTime(2026, 7, 10),
            Status = " p "
        });

        Assert.NotNull(response);
        Assert.True(response.Retorno);
        Assert.Equal(1, response.IdTitulo);
        Assert.Equal("Titulo original", response.Titulo);
        Assert.Equal("Descricao original", response.Descricao);
        Assert.Equal(new DateTime(2026, 7, 10), response.Vencimento);
        Assert.Equal('P', response.Status);

        var entity = await context.Titulos.SingleAsync();
        Assert.Equal(new DateTime(2026, 7, 10), entity.Vencimento);
        Assert.Equal('P', entity.Status);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AlterarTituloAsync_DevePermitirStatusNuloQuandoStatusNaoInformado(string? status)
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel
        {
            IdTitulo = 1,
            Titulo = "Titulo",
            Descricao = "Descricao",
            Status = 'A'
        });
        var repository = new TituloRepository(context);

        var response = await repository.AlterarTituloAsync(new TituloModelEditExclusao
        {
            IdTitulo = 1,
            Status = status
        });

        Assert.NotNull(response);
        Assert.Null(response.Status);
        Assert.Null((await context.Titulos.SingleAsync()).Status);
    }

    [Fact]
    public async Task AlterarTituloAsync_QuandoTituloNaoExiste_DeveRetornarNulo()
    {
        await using var context = CriarContexto();
        var repository = new TituloRepository(context);

        var response = await repository.AlterarTituloAsync(new TituloModelEditExclusao
        {
            IdTitulo = 999,
            Status = "A"
        });

        Assert.Null(response);
        Assert.Empty(context.Titulos);
    }

    [Fact]
    public async Task ApagarTituloAsync_QuandoTituloExiste_DeveRemoverERetornarTrue()
    {
        await using var context = CriarContexto();
        await SeedAsync(context,
            new TituloModel { IdTitulo = 1, Titulo = "Remover", Descricao = "Descricao remover", Status = 'A' },
            new TituloModel { IdTitulo = 2, Titulo = "Manter", Descricao = "Descricao manter", Status = 'P' });
        var repository = new TituloRepository(context);

        var result = await repository.ApagarTituloAsync(new TituloModelEditExclusao
        {
            IdTitulo = 1,
            Status = "A"
        });

        Assert.True(result);
        var entity = await context.Titulos.SingleAsync();
        Assert.Equal(2, entity.IdTitulo);
    }

    [Fact]
    public async Task ApagarTituloAsync_QuandoTituloNaoExiste_DeveRetornarFalse()
    {
        await using var context = CriarContexto();
        await SeedAsync(context, new TituloModel { IdTitulo = 1, Titulo = "Titulo", Descricao = "Descricao", Status = 'A' });
        var repository = new TituloRepository(context);

        var result = await repository.ApagarTituloAsync(new TituloModelEditExclusao
        {
            IdTitulo = 999,
            Status = "A"
        });

        Assert.False(result);
        Assert.Equal(1, await context.Titulos.CountAsync());
    }

    private static SadaDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SadaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SadaDbContext(options);
    }

    private static async Task SeedAsync(SadaDbContext context, params TituloModel[] titulos)
    {
        await context.Titulos.AddRangeAsync(titulos);
        await context.SaveChangesAsync();
    }
}
