using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using Sada.Application.Controllers;

namespace Sada.Application.Test;

public sealed class TituloControllerTests
{
    [Theory]
    [InlineData("P")]
    [InlineData("C")]
    public async Task CadastrarTitulo_ComDadosValidos_DeveCadastrarERetornarCreated(string status)
    {
        var business = new FakeTituloBusiness();
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);
        var request = CriarRequest(status: status);

        var result = await controller.CadastrarTitulo(request);

        Assert.IsType<CreatedResult>(result);
        Assert.Equal(1, business.CadastrarTituloCalls);
        Assert.Same(request, business.LastRequest);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Information);
    }

    [Theory]
    [InlineData(null, "P")]
    [InlineData("", "P")]
    [InlineData("Descricao", null)]
    [InlineData("Descricao", "")]
    [InlineData("Descricao", "A")]
    [InlineData("Descricao", "p")]
    public async Task CadastrarTitulo_ComDadosInvalidos_DeveRetornarBadRequestSemCadastrar(
        string? descricao,
        string? status)
    {
        var business = new FakeTituloBusiness();
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);
        var request = CriarRequest(descricao: descricao!, status: status);

        var result = await controller.CadastrarTitulo(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Dados do título inválidos.", badRequest.Value);
        Assert.Equal(0, business.CadastrarTituloCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Critical);
    }

    [Fact]
    public async Task ListarTitulos_DeveRetornarOkComTodosOsTitulos()
    {
        var expected = new List<TituloModelResponse>
        {
            CriarResponse(1),
            CriarResponse(2)
        };
        var business = new FakeTituloBusiness
        {
            ListTitulosResult = expected
        };
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);

        var result = await controller.ListarTitulos();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Equal(1, business.ListTitulosCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Information);
    }

    [Fact]
    public async Task ListarTitulosByStatusVencimento_ComDadosValidos_DeveRetornarOkComListaFiltrada()
    {
        var vencimento = new DateTime(2026, 6, 1);
        var expected = new List<TituloModelResponse> { CriarResponse(1) };
        var business = new FakeTituloBusiness
        {
            ListTitulosFilteredResult = expected
        };
        var controller = CriarController(business);
        var request = CriarEditExclusao(status: "P", vencimento: vencimento);

        var result = await controller.ListarTitulosByStatusVencimento(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Equal(1, business.ListTitulosFilteredCalls);
        Assert.Equal("P", business.LastStatus);
        Assert.Equal(vencimento, business.LastVencimento);
    }

    [Theory]
    [InlineData(null, "2026-06-01")]
    [InlineData("", "2026-06-01")]
    [InlineData("A", "2026-06-01")]
    [InlineData("P", null)]
    public async Task ListarTitulosByStatusVencimento_ComDadosInvalidos_DeveRetornarBadRequestSemConsultar(
        string? status,
        string? vencimentoText)
    {
        var business = new FakeTituloBusiness();
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);
        var request = CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText));

        var result = await controller.ListarTitulosByStatusVencimento(request);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ListarTitulosByStatusVencimento() - Dados do Título são inválidos.", badRequest.Value);
        Assert.Equal(0, business.ListTitulosFilteredCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Critical);
    }

    [Fact]
    public async Task AlterarTitulo_ComDadosValidosERegistroExistente_DeveRetornarOk()
    {
        var response = CriarResponse(10);
        var business = new FakeTituloBusiness
        {
            AlterarTituloResult = response
        };
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);
        var request = CriarEditExclusao(idTitulo: 10);

        var result = await controller.AlterarTitulo(request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        Assert.Equal(1, business.AlterarTituloCalls);
        Assert.Same(request, business.LastEditRequest);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Information);
    }

    [Fact]
    public async Task AlterarTitulo_QuandoBusinessRetornaNulo_DeveRetornarNotFound()
    {
        var business = new FakeTituloBusiness
        {
            AlterarTituloResult = null
        };
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);

        var result = await controller.AlterarTitulo(CriarEditExclusao());

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Null(notFound.Value);
        Assert.Equal(1, business.AlterarTituloCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Warning);
    }

    [Theory]
    [InlineData(null, "2026-06-01")]
    [InlineData("", "2026-06-01")]
    [InlineData("A", "2026-06-01")]
    [InlineData("P", null)]
    public async Task AlterarTitulo_ComDadosInvalidos_DeveRetornarBadRequestSemAlterar(
        string? status,
        string? vencimentoText)
    {
        var business = new FakeTituloBusiness();
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);

        var result = await controller.AlterarTitulo(CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText)));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("AlterarTitulo() - Dados do Título são inválidos.", badRequest.Value);
        Assert.Equal(0, business.AlterarTituloCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Critical);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ApagarTitulo_ComDadosValidos_DeveRetornarResultadoConformeBusiness(bool apagou)
    {
        var business = new FakeTituloBusiness
        {
            ApagarTituloResult = apagou
        };
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);
        var request = CriarEditExclusao(idTitulo: 3);

        var result = await controller.ApagarTitulo(request);

        Assert.Equal(1, business.ApagarTituloCalls);
        Assert.Same(request, business.LastDeleteRequest);

        if (apagou)
        {
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)ok.Value!);
            Assert.Contains(logger.Entries, item => item.Level == LogLevel.Information);
        }
        else
        {
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("ApagarTitulo() - Registro não encontrado.", notFound.Value);
            Assert.Contains(logger.Entries, item => item.Level == LogLevel.Warning);
        }
    }

    [Theory]
    [InlineData(null, "2026-06-01")]
    [InlineData("", "2026-06-01")]
    [InlineData("A", "2026-06-01")]
    [InlineData("P", null)]
    public async Task ApagarTitulo_ComDadosInvalidos_DeveRetornarBadRequestSemApagar(
        string? status,
        string? vencimentoText)
    {
        var business = new FakeTituloBusiness();
        var logger = new TestLogger<TituloController>();
        var controller = CriarController(business, logger);

        var result = await controller.ApagarTitulo(CriarEditExclusaoComVencimentoOpcional(status, ParseDate(vencimentoText)));

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("ApagarTitulo() - Dados do Título são inválidos.", badRequest.Value);
        Assert.Equal(0, business.ApagarTituloCalls);
        Assert.Contains(logger.Entries, item => item.Level == LogLevel.Critical);
    }

    [Fact]
    public void Index_DeveRetornarView()
    {
        var controller = CriarController();

        var result = controller.Index();

        Assert.IsType<ViewResult>(result);
    }

    private static TituloController CriarController(
        FakeTituloBusiness? business = null,
        TestLogger<TituloController>? logger = null)
    {
        return new TituloController(
            logger ?? new TestLogger<TituloController>(),
            business ?? new FakeTituloBusiness());
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
            Vencimento = new DateTime(2026, 6, 1),
            Status = status
        };
    }

    private static TituloModelEditExclusao CriarEditExclusao(
        string? status = "P",
        DateTime? vencimento = null,
        int idTitulo = 1)
    {
        return new TituloModelEditExclusao
        {
            IdTitulo = idTitulo,
            Status = status,
            Vencimento = vencimento ?? new DateTime(2026, 6, 1)
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
            Vencimento = new DateTime(2026, 6, 1),
            Status = 'P',
            Retorno = true
        };
    }

    private sealed class FakeTituloBusiness : ITitulo
    {
        public List<TituloModelResponse> ListTitulosResult { get; set; } = [];
        public List<TituloModelResponse> ListTitulosFilteredResult { get; set; } = [];
        public TituloModelResponse CadastrarTituloResult { get; set; } = new();
        public TituloModelResponse? AlterarTituloResult { get; set; } = new();
        public bool ApagarTituloResult { get; set; }
        public int ListTitulosCalls { get; private set; }
        public int ListTitulosFilteredCalls { get; private set; }
        public int CadastrarTituloCalls { get; private set; }
        public int AlterarTituloCalls { get; private set; }
        public int ApagarTituloCalls { get; private set; }
        public string? LastStatus { get; private set; }
        public DateTime? LastVencimento { get; private set; }
        public TituloModelRequest? LastRequest { get; private set; }
        public TituloModelEditExclusao? LastEditRequest { get; private set; }
        public TituloModelEditExclusao? LastDeleteRequest { get; private set; }

        public Task<List<TituloModelResponse>> ListTitulos()
        {
            ListTitulosCalls++;
            return Task.FromResult(ListTitulosResult);
        }

        public Task<List<TituloModelResponse>> ListTitulos(string? status, DateTime? vencimento)
        {
            ListTitulosFilteredCalls++;
            LastStatus = status;
            LastVencimento = vencimento;
            return Task.FromResult(ListTitulosFilteredResult);
        }

        public Task<TituloModelResponse> CadastrarTitulo(TituloModelRequest model)
        {
            CadastrarTituloCalls++;
            LastRequest = model;
            return Task.FromResult(CadastrarTituloResult);
        }

        public Task<TituloModelResponse> AlterarTitulo(TituloModelEditExclusao model)
        {
            AlterarTituloCalls++;
            LastEditRequest = model;
            return Task.FromResult(AlterarTituloResult!);
        }

        public Task<bool> ApagarTitulo(TituloModelEditExclusao model)
        {
            ApagarTituloCalls++;
            LastDeleteRequest = model;
            return Task.FromResult(ApagarTituloResult);
        }
    }
}
