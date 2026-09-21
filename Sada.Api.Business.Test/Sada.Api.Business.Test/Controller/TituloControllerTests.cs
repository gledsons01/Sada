using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;

namespace Sada.Api.Business.Test.Controller;

public sealed class TituloControllerTests
{
    private readonly Mock<ILogger<TituloController>> _loggerMock = new();
    private readonly Mock<ITitulo> _tituloBusinessMock = new();

    [Theory]
    [InlineData("P")]
    [InlineData("C")]
    public async Task CadastrarTituloAsync_ComDadosValidos_DeveCadastrarERetornarCreated(string status)
    {
        var request = CriarRequest(status: status);
        var response = CriarResponse(1);

        _tituloBusinessMock
            .Setup(business => business.CadastrarTituloAsync(request))
            .ReturnsAsync(response);

        var controller = CriarController();

        var result = await controller.CadastrarTitulo(request);

        Assert.IsType<CreatedResult>(result);
        _tituloBusinessMock.Verify(business => business.CadastrarTituloAsync(request), Times.Once);
    }

    [Theory]
    [InlineData(null, "P")]
    [InlineData("", "P")]
    [InlineData("Descricao", null)]
    [InlineData("Descricao", "")]
    [InlineData("Descricao", "A")]
    [InlineData("Descricao", "p")]
    public async Task CadastrarTituloAsync_ComDadosInvalidos_DeveRetornarBadRequestSemCadastrar(
        string? descricao,
        string? status)
    {
        var request = CriarRequest(descricao: descricao!, status: status);
        var controller = CriarController();

        var result = await controller.CadastrarTitulo(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados do título inválidos.", badRequest.Value);
        _tituloBusinessMock.Verify(business => business.CadastrarTituloAsync(It.IsAny<TituloModelRequest>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarTituloAsync_QuandoBusinessLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarRequest();
        var exception = new InvalidOperationException("Falha ao cadastrar titulo");

        _tituloBusinessMock
            .Setup(business => business.CadastrarTituloAsync(request))
            .ThrowsAsync(exception);

        var controller = CriarController();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.CadastrarTitulo(request));

        Assert.Same(exception, result);
        _tituloBusinessMock.Verify(business => business.CadastrarTituloAsync(request), Times.Once);
    }

    [Fact]
    public async Task ListarTitulos_DeveRetornarOkComTodosOsTitulos()
    {
        var titulos = new List<TituloModelResponse>
        {
            CriarResponse(1),
            CriarResponse(2)
        };

        _tituloBusinessMock
            .Setup(business => business.ListTitulosAsync())
            .ReturnsAsync(titulos);

        var controller = CriarController();

        var result = await controller.ListarTitulosAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(titulos, ok.Value);
        _tituloBusinessMock.Verify(business => business.ListTitulosAsync(), Times.Once);
    }

    [Fact]
    public async Task ListarTitulos_QuandoBusinessRetornaListaVazia_DeveRetornarOkComListaVazia()
    {
        var titulos = new List<TituloModelResponse>();

        _tituloBusinessMock
            .Setup(business => business.ListTitulosAsync())
            .ReturnsAsync(titulos);

        var controller = CriarController();

        var result = await controller.ListarTitulosAsync();

        var ok = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<List<TituloModelResponse>>(ok.Value);
        Assert.Empty(value);
        _tituloBusinessMock.Verify(business => business.ListTitulosAsync(), Times.Once);
    }

    [Fact]
    public async Task ListarTitulos_QuandoBusinessLancaExcecao_DeveRelancarExcecao()
    {
        var exception = new InvalidOperationException("Falha ao listar titulos");

        _tituloBusinessMock
            .Setup(business => business.ListTitulosAsync())
            .ThrowsAsync(exception);

        var controller = CriarController();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.ListarTitulosAsync());

        Assert.Same(exception, result);
        _tituloBusinessMock.Verify(business => business.ListTitulosAsync(), Times.Once);
    }

    [Theory]
    [InlineData("P")]
    [InlineData("C")]
    public async Task ListarTitulosByStatusVencimento_ComDadosValidos_DeveRetornarOkComListaFiltrada(string status)
    {
        var vencimento = new DateTime(2026, 7, 10);
        var request = CriarEditExclusao(status: status, vencimento: vencimento);
        var titulos = new List<TituloModelResponse> { CriarResponse(3) };

        _tituloBusinessMock
            .Setup(business => business.ListTitulosAsync(status, vencimento))
            .ReturnsAsync(titulos);

        var controller = CriarController();

        var result = await controller.ListarTitulosByStatusVencimento(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(titulos, ok.Value);
        _tituloBusinessMock.Verify(business => business.ListTitulosAsync(status, vencimento), Times.Once);
    }

    [Theory]
    [InlineData(null, "2026-07-10")]
    [InlineData("", "2026-07-10")]
    [InlineData("A", "2026-07-10")]
    [InlineData("p", "2026-07-10")]
    [InlineData("P", null)]
    public async Task ListarTitulosByStatusVencimento_ComDadosInvalidos_DeveRetornarBadRequestSemConsultar(
        string? status,
        string? vencimentoText)
    {
        var request = CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText));
        var controller = CriarController();

        var result = await controller.ListarTitulosByStatusVencimento(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ListarTitulosByStatusVencimento() - Dados do Título são inválidos.", badRequest.Value);
        _tituloBusinessMock.Verify(
            business => business.ListTitulosAsync(It.IsAny<string?>(), It.IsAny<DateTime?>()),
            Times.Never);
    }

    [Fact]
    public async Task ListarTitulosByStatusVencimento_QuandoBusinessLancaExcecao_DeveRelancarExcecao()
    {
        var vencimento = new DateTime(2026, 7, 10);
        var request = CriarEditExclusao(status: "P", vencimento: vencimento);
        var exception = new InvalidOperationException("Falha ao listar titulos filtrados");

        _tituloBusinessMock
            .Setup(business => business.ListTitulosAsync("P", vencimento))
            .ThrowsAsync(exception);

        var controller = CriarController();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            controller.ListarTitulosByStatusVencimento(request));

        Assert.Same(exception, result);
        _tituloBusinessMock.Verify(business => business.ListTitulosAsync("P", vencimento), Times.Once);
    }

    [Theory]
    [InlineData("P")]
    [InlineData("C")]
    public async Task AlterarTituloAsync_ComDadosValidosERegistroExistente_DeveRetornarOk(string status)
    {
        var request = CriarEditExclusao(idTitulo: 10, status: status);
        var response = CriarResponse(request.IdTitulo);

        _tituloBusinessMock
            .Setup(business => business.AlterarTituloAsync(request))
            .ReturnsAsync(response);

        var controller = CriarController();

        var result = await controller.AlterarTitulo(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        _tituloBusinessMock.Verify(business => business.AlterarTituloAsync(request), Times.Once);
    }

    [Fact]
    public async Task AlterarTituloAsync_QuandoBusinessRetornaNull_DeveRetornarNotFound()
    {
        var request = CriarEditExclusao(idTitulo: 11);

        _tituloBusinessMock
            .Setup(business => business.AlterarTituloAsync(request))
            .ReturnsAsync((TituloModelResponse)null!);

        var controller = CriarController();

        var result = await controller.AlterarTitulo(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Null(notFound.Value);
        _tituloBusinessMock.Verify(business => business.AlterarTituloAsync(request), Times.Once);
    }

    [Theory]
    [InlineData(null, "2026-07-10")]
    [InlineData("", "2026-07-10")]
    [InlineData("A", "2026-07-10")]
    [InlineData("p", "2026-07-10")]
    [InlineData("P", null)]
    public async Task AlterarTituloAsync_ComDadosInvalidos_DeveRetornarBadRequestSemAlterar(
        string? status,
        string? vencimentoText)
    {
        var request = CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText));
        var controller = CriarController();

        var result = await controller.AlterarTitulo(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("AlterarTitulo() - Dados do Título são inválidos.", badRequest.Value);
        _tituloBusinessMock.Verify(
            business => business.AlterarTituloAsync(It.IsAny<TituloModelEditExclusao>()),
            Times.Never);
    }

    [Fact]
    public async Task AlterarTituloAsync_QuandoBusinessLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarEditExclusao();
        var exception = new InvalidOperationException("Falha ao alterar titulo");

        _tituloBusinessMock
            .Setup(business => business.AlterarTituloAsync(request))
            .ThrowsAsync(exception);

        var controller = CriarController();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.AlterarTitulo(request));

        Assert.Same(exception, result);
        _tituloBusinessMock.Verify(business => business.AlterarTituloAsync(request), Times.Once);
    }

    [Theory]
    [InlineData("P")]
    [InlineData("C")]
    public async Task ApagarTituloAsync_ComDadosValidosERegistroExistente_DeveRetornarOk(string status)
    {
        var request = CriarEditExclusao(idTitulo: 12, status: status);

        _tituloBusinessMock
            .Setup(business => business.ApagarTituloAsync(request))
            .ReturnsAsync(true);

        var controller = CriarController();

        var result = await controller.ApagarTitulo(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)ok.Value!);
        _tituloBusinessMock.Verify(business => business.ApagarTituloAsync(request), Times.Once);
    }

    [Fact]
    public async Task ApagarTituloAsync_QuandoBusinessRetornaFalse_DeveRetornarNotFound()
    {
        var request = CriarEditExclusao(idTitulo: 13);

        _tituloBusinessMock
            .Setup(business => business.ApagarTituloAsync(request))
            .ReturnsAsync(false);

        var controller = CriarController();

        var result = await controller.ApagarTitulo(request);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("ApagarTitulo() - Registro não encontrado.", notFound.Value);
        _tituloBusinessMock.Verify(business => business.ApagarTituloAsync(request), Times.Once);
    }

    [Theory]
    [InlineData(null, "2026-07-10")]
    [InlineData("", "2026-07-10")]
    [InlineData("A", "2026-07-10")]
    [InlineData("p", "2026-07-10")]
    [InlineData("P", null)]
    public async Task ApagarTituloAsync_ComDadosInvalidos_DeveRetornarBadRequestSemApagar(
        string? status,
        string? vencimentoText)
    {
        var request = CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText));
        var controller = CriarController();

        var result = await controller.ApagarTitulo(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ApagarTitulo() - Dados do Título são inválidos.", badRequest.Value);
        _tituloBusinessMock.Verify(
            business => business.ApagarTituloAsync(It.IsAny<TituloModelEditExclusao>()),
            Times.Never);
    }

    [Fact]
    public async Task ApagarTituloAsync_QuandoBusinessLancaExcecao_DeveRelancarExcecao()
    {
        var request = CriarEditExclusao();
        var exception = new InvalidOperationException("Falha ao apagar titulo");

        _tituloBusinessMock
            .Setup(business => business.ApagarTituloAsync(request))
            .ThrowsAsync(exception);

        var controller = CriarController();

        var result = await Assert.ThrowsAsync<InvalidOperationException>(() => controller.ApagarTitulo(request));

        Assert.Same(exception, result);
        _tituloBusinessMock.Verify(business => business.ApagarTituloAsync(request), Times.Once);
    }

    [Fact]
    public void Index_DeveRetornarView()
    {
        var controller = CriarController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    private TituloController CriarController()
    {
        return new TituloController(_loggerMock.Object, _tituloBusinessMock.Object);
    }

    private static TituloModelRequest CriarRequest(
        string titulo = "Titulo",
        string descricao = "Descricao",
        string? status = "P")
    {
        return new TituloModelRequest
        {
            Titulo = titulo,
            Descricao = descricao,
            Vencimento = new DateTime(2026, 7, 10),
            Status = status
        };
    }

    private static TituloModelEditExclusao CriarEditExclusao(
        int idTitulo = 1,
        string? status = "P",
        DateTime? vencimento = null)
    {
        return new TituloModelEditExclusao
        {
            IdTitulo = idTitulo,
            Status = status,
            Vencimento = vencimento ?? new DateTime(2026, 7, 10)
        };
    }

    private static TituloModelEditExclusao CriarEditExclusaoComVencimentoOpcional(
        string? status,
        DateTime? vencimento,
        int idTitulo = 1)
    {
        return new TituloModelEditExclusao
        {
            IdTitulo = idTitulo,
            Status = status,
            Vencimento = vencimento
        };
    }

    private static DateTime? ParseDate(string? value)
    {
        return value is null ? null : DateTime.Parse(value);
    }

    private static TituloModelResponse CriarResponse(int idTitulo)
    {
        return new TituloModelResponse
        {
            IdTitulo = idTitulo,
            Titulo = $"Titulo {idTitulo}",
            Descricao = $"Descricao {idTitulo}",
            Vencimento = new DateTime(2026, 7, 10),
            Status = 'P',
            Retorno = true
        };
    }
}
