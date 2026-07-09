using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Test.Business;

public sealed class TituloBusinessTests
{
    private readonly Mock<ILogger<Titulo>> _loggerMock = new();
    private readonly Mock<ITituloRepository> _tituloRepositoryMock = new();

    [Fact]
    public void Titulo_DeveImplementarInterfaceITitulo()
    {
        var service = CriarServico();

        Assert.IsAssignableFrom<ITitulo>(service);
    }

    [Fact]
    public async Task ListTitulos_DeveRetornarTodosOsTitulosDoRepositorio()
    {
        var titulos = new List<TituloModelResponse>
        {
            CriarResponse(1, "Titulo 1"),
            CriarResponse(2, "Titulo 2")
        };

        _tituloRepositoryMock
            .Setup(repository => repository.ListarTitulosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(titulos);

        var service = CriarServico();

        var result = await service.ListTitulos();

        Assert.Same(titulos, result);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(
                It.IsAny<string?>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ListTitulos_QuandoRepositorioRetornaListaVazia_DeveRetornarListaVazia()
    {
        var titulos = new List<TituloModelResponse>();

        _tituloRepositoryMock
            .Setup(repository => repository.ListarTitulosAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(titulos);

        var service = CriarServico();

        var result = await service.ListTitulos();

        Assert.Empty(result);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListTitulos_ComFiltros_DeveRepassarFiltrosERetornarTitulosDoRepositorio()
    {
        const string status = "A";
        var vencimento = new DateTime(2026, 7, 10);
        var titulos = new List<TituloModelResponse>
        {
            CriarResponse(3, "Titulo filtrado")
        };

        _tituloRepositoryMock
            .Setup(repository => repository.ListarTitulosAsync(status, vencimento, It.IsAny<CancellationToken>()))
            .ReturnsAsync(titulos);

        var service = CriarServico();

        var result = await service.ListTitulos(status, vencimento);

        Assert.Same(titulos, result);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(status, vencimento, It.IsAny<CancellationToken>()),
            Times.Once);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("P")]
    public async Task ListTitulos_ComStatusOpcional_DeveRepassarStatusRecebido(string? status)
    {
        DateTime? vencimento = null;

        _tituloRepositoryMock
            .Setup(repository => repository.ListarTitulosAsync(status, vencimento, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TituloModelResponse>());

        var service = CriarServico();

        var result = await service.ListTitulos(status, vencimento);

        Assert.Empty(result);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(status, vencimento, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ListTitulos_ComVencimentoNulo_DeveRepassarVencimentoNulo()
    {
        const string status = "A";

        _tituloRepositoryMock
            .Setup(repository => repository.ListarTitulosAsync(status, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TituloModelResponse>());

        var service = CriarServico();

        var result = await service.ListTitulos(status, null);

        Assert.Empty(result);
        _tituloRepositoryMock.Verify(
            repository => repository.ListarTitulosAsync(status, null, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CadastrarTitulo_QuandoRepositorioRetornaSucesso_DeveRetornarResposta()
    {
        var request = CriarRequest("Novo titulo");
        var response = CriarResponse(4, request.Titulo);

        _tituloRepositoryMock
            .Setup(repository => repository.IncluirTituloAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.CadastrarTitulo(request);

        Assert.Same(response, result);
        _tituloRepositoryMock.Verify(
            repository => repository.IncluirTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CadastrarTitulo_QuandoRepositorioLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarRequest("Titulo com erro");
        var exception = new InvalidOperationException("Falha no banco");

        _tituloRepositoryMock
            .Setup(repository => repository.IncluirTituloAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var service = CriarServico();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => service.CadastrarTitulo(request));

        Assert.Same(exception, result);
        _tituloRepositoryMock.Verify(
            repository => repository.IncluirTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AlterarTitulo_QuandoTituloExiste_DeveRetornarResposta()
    {
        var request = CriarEditExclusao(5, "P");
        var response = CriarResponse(request.IdTitulo, "Titulo alterado");

        _tituloRepositoryMock
            .Setup(repository => repository.AlterarTituloAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var service = CriarServico();

        var result = await service.AlterarTitulo(request);

        Assert.Same(response, result);
        _tituloRepositoryMock.Verify(
            repository => repository.AlterarTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task AlterarTitulo_QuandoRepositorioRetornaNull_DeveLancarKeyNotFoundException()
    {
        var request = CriarEditExclusao(99, "P");

        _tituloRepositoryMock
            .Setup(repository => repository.AlterarTituloAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TituloModelResponse?)null);

        var service = CriarServico();

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => service.AlterarTitulo(request));

        Assert.Equal("Titulo com ID 99 não encontrado.", exception.Message);
        _tituloRepositoryMock.Verify(
            repository => repository.AlterarTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ApagarTitulo_DeveRetornarResultadoDoRepositorio(bool repositoryResult)
    {
        var request = CriarEditExclusao(6, "A");

        _tituloRepositoryMock
            .Setup(repository => repository.ApagarTituloAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(repositoryResult);

        var service = CriarServico();

        var result = await service.ApagarTitulo(request);

        Assert.Equal(repositoryResult, result);
        _tituloRepositoryMock.Verify(
            repository => repository.ApagarTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ApagarTitulo_QuandoRepositorioLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarEditExclusao(7, "A");
        var exception = new InvalidOperationException("Falha ao apagar titulo");

        _tituloRepositoryMock
            .Setup(repository => repository.ApagarTituloAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception);

        var service = CriarServico();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApagarTitulo(request));

        Assert.Same(exception, result);
        _tituloRepositoryMock.Verify(
            repository => repository.ApagarTituloAsync(request, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private Titulo CriarServico()
    {
        return new Titulo(_loggerMock.Object, _tituloRepositoryMock.Object);
    }

    private static TituloModelRequest CriarRequest(string titulo)
    {
        return new TituloModelRequest
        {
            Titulo = titulo,
            Descricao = $"Descricao {titulo}",
            Vencimento = new DateTime(2026, 7, 10),
            Status = "A"
        };
    }

    private static TituloModelEditExclusao CriarEditExclusao(int idTitulo, string status)
    {
        return new TituloModelEditExclusao
        {
            IdTitulo = idTitulo,
            Vencimento = new DateTime(2026, 7, 10),
            Status = status
        };
    }

    private static TituloModelResponse CriarResponse(int idTitulo, string titulo)
    {
        return new TituloModelResponse
        {
            IdTitulo = idTitulo,
            Titulo = titulo,
            Descricao = $"Descricao {idTitulo}",
            Vencimento = new DateTime(2026, 7, 10),
            Status = 'A',
            Retorno = true
        };
    }
}
