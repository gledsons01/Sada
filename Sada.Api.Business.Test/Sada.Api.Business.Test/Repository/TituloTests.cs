using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Test.Repository;
public sealed class TituloTests
{
    [Fact]
    public void Titulo_DeveImplementarInterfaceITitulo()
    {
        var service = CriarServico();

        Assert.IsAssignableFrom<ITitulo>(service);
    }

    [Fact]
    public async Task ListTitulos_DeveRetornarTodosOsTitulosERegistrarQuantidade()
    {
        var repository = new FakeTituloRepository
        {
            ListarTitulosResult =
            [
                CriarResponse(1, "Titulo 1"),
                CriarResponse(2, "Titulo 2")
            ]
        };

        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.ListTitulos();

        Assert.Same(repository.ListarTitulosResult, result);
        Assert.Equal(1, repository.ListarTitulosCalls);
        Assert.Equal(0, repository.ListarTitulosFiltradoCalls);
        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("Listagem de Registros Cadastrados", log.Message);
        Assert.Contains("2", log.Message);
    }

    [Fact]
    public async Task ListTitulos_QuandoRepositorioRetornaListaVazia_DeveRetornarListaVaziaELogarZero()
    {
        var repository = new FakeTituloRepository
        {
            ListarTitulosResult = []
        };
        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.ListTitulos();

        Assert.Empty(result);
        Assert.Equal(1, repository.ListarTitulosCalls);
        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("0", log.Message);
    }

    [Fact]
    public async Task ListTitulos_ComFiltros_DeveRepassarParametrosRetornarResultadoERegistrarQuantidade()
    {
        var vencimento = new DateTime(2026, 6, 1);
        var repository = new FakeTituloRepository
        {
            ListarTitulosFiltradoResult = [CriarResponse(10, "Filtrado")]
        };
        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.ListTitulos("A", vencimento);

        Assert.Same(repository.ListarTitulosFiltradoResult, result);
        Assert.Equal(1, repository.ListarTitulosFiltradoCalls);
        Assert.Equal(0, repository.ListarTitulosCalls);
        Assert.Equal("A", repository.LastStatus);
        Assert.Equal(vencimento, repository.LastVencimento);

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("Listagem de Registros Filtrados", log.Message);
        Assert.Contains("1", log.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ListTitulos_ComStatusOpcional_DeveRepassarStatusComoRecebido(string? status)
    {
        var repository = new FakeTituloRepository
        {
            ListarTitulosFiltradoResult = []
        };
        var service = CriarServico(repository);

        await service.ListTitulos(status, null);

        Assert.Equal(status, repository.LastStatus);
        Assert.Null(repository.LastVencimento);
    }

    [Fact]
    public async Task CadastrarTitulo_QuandoRepositorioRetornaSucesso_DeveRetornarRespostaERegistrarLog()
    {
        var request = new TituloModelRequest
        {
            Titulo = "Novo titulo",
            Descricao = "Nova descricao",
            Vencimento = new DateTime(2026, 6, 1),
            Status = "A"
        };
        var response = CriarResponse(5, request.Titulo);
        var repository = new FakeTituloRepository
        {
            IncluirTituloResult = response
        };
        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.CadastrarTitulo(request);

        Assert.Same(response, result);
        Assert.Equal(1, repository.IncluirTituloCalls);
        Assert.Same(request, repository.LastRequest);
        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("Cadastro de Titulo Novo titulo efetuado com sucesso", log.Message);
    }

    [Fact]
    public async Task CadastrarTitulo_QuandoRepositorioLancaExcecao_DeveLogarErroERelancar()
    {
        var request = new TituloModelRequest
        {
            Titulo = "Titulo com erro",
            Descricao = "Descricao"
        };
        var exception = new InvalidOperationException("Falha no banco");
        var repository = new FakeTituloRepository
        {
            IncluirTituloException = exception
        };
        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var thrown = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CadastrarTitulo(request));

        Assert.Same(exception, thrown);
        Assert.Equal(1, repository.IncluirTituloCalls);
        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Same(exception, log.Exception);
        Assert.Contains("Erro ao cadastrar titulo Titulo com erro", log.Message);
    }

    [Fact]
    public async Task AlterarTitulo_QuandoTituloExiste_DeveRetornarRespostaERegistrarLog()
    {
        var request = new TituloModelEditExclusao
        {
            IdTitulo = 7,
            Vencimento = new DateTime(2026, 7, 1),
            Status = "P"
        };

        var response = CriarResponse(7, "Alterado");
        var repository = new FakeTituloRepository
        {
            AlterarTituloResult = response
        };

        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.AlterarTitulo(request);

        Assert.Same(response, result);
        Assert.Equal(1, repository.AlterarTituloCalls);
        Assert.Same(request, repository.LastEditRequest);

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("Titulo alterado. ID: 7", log.Message);
    }

    [Fact]
    public async Task AlterarTitulo_QuandoTituloNaoExiste_DeveLogarAvisoELancarKeyNotFoundException()
    {
        var request = new TituloModelEditExclusao
        {
            IdTitulo = 99,
            Status = "A"
        };

        var repository = new FakeTituloRepository
        {
            AlterarTituloResult = null
        };

        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AlterarTitulo(request));

        Assert.Equal("Titulo com ID 99 não encontrado.", exception.Message);
        Assert.Equal(1, repository.AlterarTituloCalls);

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, log.Level);
        Assert.Contains("Titulo não encontrado para alteração. ID: 99", log.Message);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ApagarTitulo_DeveRetornarResultadoDoRepositorioERegistrarLog(bool repositoryResult)
    {
        var request = new TituloModelEditExclusao
        {
            IdTitulo = 12,
            Status = "A"
        };

        var repository = new FakeTituloRepository
        {
            ApagarTituloResult = repositoryResult
        };

        var logger = new TestLogger<Sada.Api.Business.Titulo>();
        var service = CriarServico(repository, logger);

        var result = await service.ApagarTitulo(request);

        Assert.Equal(repositoryResult, result);
        Assert.Equal(1, repository.ApagarTituloCalls);
        Assert.Same(request, repository.LastDeleteRequest);

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, log.Level);
        Assert.Contains("Titulo apagado. ID: 12", log.Message);
    }

    private static Sada.Api.Business.Titulo CriarServico(FakeTituloRepository? repository = null, TestLogger<Sada.Api.Business.Titulo>? logger = null)
    {
        return new Sada.Api.Business.Titulo(
            logger ?? new TestLogger<Sada.Api.Business.Titulo>(),
            repository ?? new FakeTituloRepository());
    }

    private static TituloModelResponse CriarResponse(int idTitulo, string titulo)
    {
        return new TituloModelResponse
        {
            IdTitulo = idTitulo,
            Titulo = titulo,
            Descricao = $"Descricao {idTitulo}",
            Vencimento = new DateTime(2026, 6, idTitulo),
            Status = 'A',
            Retorno = true
        };
    }

    private sealed class FakeTituloRepository : ITituloRepository
    {
        public List<TituloModelResponse> ListarTitulosResult { get; set; } = [];
        public List<TituloModelResponse> ListarTitulosFiltradoResult { get; set; } = [];
        public TituloModelResponse IncluirTituloResult { get; set; } = new();
        public Exception? IncluirTituloException { get; set; }
        public TituloModelResponse? AlterarTituloResult { get; set; } = new();
        public bool ApagarTituloResult { get; set; }
        public int ListarTitulosCalls { get; private set; }
        public int ListarTitulosFiltradoCalls { get; private set; }
        public int IncluirTituloCalls { get; private set; }
        public int AlterarTituloCalls { get; private set; }
        public int ApagarTituloCalls { get; private set; }
        public string? LastStatus { get; private set; }
        public DateTime? LastVencimento { get; private set; }
        public TituloModelRequest? LastRequest { get; private set; }
        public TituloModelEditExclusao? LastEditRequest { get; private set; }
        public TituloModelEditExclusao? LastDeleteRequest { get; private set; }

        public Task<TituloModelResponse> IncluirTituloAsync(
            TituloModelRequest model,
            CancellationToken cancellationToken = default)
        {
            IncluirTituloCalls++;
            LastRequest = model;

            if (IncluirTituloException is not null)
            {
                throw IncluirTituloException;
            }

            return Task.FromResult(IncluirTituloResult);
        }

        public Task<List<TituloModelResponse>> ListarTitulosAsync(CancellationToken cancellationToken = default)
        {
            ListarTitulosCalls++;
            return Task.FromResult(ListarTitulosResult);
        }

        public Task<List<TituloModelResponse>> ListarTitulosAsync(
            string? status,
            DateTime? vencimento,
            CancellationToken cancellationToken = default)
        {
            ListarTitulosFiltradoCalls++;
            LastStatus = status;
            LastVencimento = vencimento;
            return Task.FromResult(ListarTitulosFiltradoResult);
        }

        public Task<bool> ApagarTituloAsync(
            TituloModelEditExclusao model,
            CancellationToken cancellationToken = default)
        {
            ApagarTituloCalls++;
            LastDeleteRequest = model;
            return Task.FromResult(ApagarTituloResult);
        }

        public Task<TituloModelResponse?> AlterarTituloAsync(
            TituloModelEditExclusao model,
            CancellationToken cancellationToken = default)
        {
            AlterarTituloCalls++;
            LastEditRequest = model;
            return Task.FromResult(AlterarTituloResult);
        }
    }

    private sealed class TestLogger<T> : ILogger<T>
    {
        public List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull
        {
            return NullScope.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add(new LogEntry(logLevel, eventId, formatter(state, exception), exception));
        }
    }

    private sealed record LogEntry(LogLevel Level, EventId EventId, string Message, Exception? Exception);

    private sealed class NullScope : IDisposable
    {
        public static readonly NullScope Instance = new();

        public void Dispose()
        {
        }
    }
}
