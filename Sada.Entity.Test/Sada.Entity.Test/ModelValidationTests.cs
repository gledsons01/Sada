using System.ComponentModel.DataAnnotations;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Entity.Test;

public sealed class ModelValidationTests
{
    [Fact]
    public void TituloModelRequest_Valido_NaoDeveGerarErroDeValidacao()
    {
        var model = new TituloModelRequest
        {
            Titulo = "Titulo",
            Descricao = "Descricao",
            Vencimento = new DateTime(2026, 6, 1),
            Status = "A"
        };

        var result = Validar(model);

        Assert.Empty(result);
    }

    [Theory]
    [InlineData(null, "Descricao")]
    [InlineData("", "Descricao")]
    [InlineData("Titulo", null)]
    [InlineData("Titulo", "")]
    public void TituloModelRequest_ComCamposObrigatoriosInvalidos_DeveGerarErroDeValidacao(
        string? titulo,
        string? descricao)
    {
        var model = new TituloModelRequest
        {
            Titulo = titulo!,
            Descricao = descricao!
        };

        var result = Validar(model);

        Assert.NotEmpty(result);
    }

    [Fact]
    public void TituloModelRequest_ComTituloMaiorQueMaximo_DeveGerarErroDeValidacao()
    {
        var model = new TituloModelRequest
        {
            Titulo = new string('T', 151),
            Descricao = "Descricao"
        };

        var result = Validar(model);

        Assert.Contains(result, item => item.MemberNames.Contains(nameof(TituloModelRequest.Titulo)));
    }

    [Fact]
    public void TituloModelRequest_ComDescricaoMaiorQueMaximo_DeveGerarErroDeValidacao()
    {
        var model = new TituloModelRequest
        {
            Titulo = "Titulo",
            Descricao = new string('D', 201)
        };

        var result = Validar(model);

        Assert.Contains(result, item => item.MemberNames.Contains(nameof(TituloModelRequest.Descricao)));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TituloModelEditExclusao_ComStatusObrigatorioInvalido_DeveGerarErroDeValidacao(string? status)
    {
        var model = new TituloModelEditExclusao
        {
            IdTitulo = 1,
            Status = status
        };

        var result = Validar(model);

        Assert.Contains(result, item => item.MemberNames.Contains(nameof(TituloModelEditExclusao.Status)));
    }

    [Fact]
    public void TituloModelEditExclusao_ComStatusInformado_NaoDeveGerarErroDeValidacao()
    {
        var model = new TituloModelEditExclusao
        {
            IdTitulo = 1,
            Status = "A"
        };

        var result = Validar(model);

        Assert.Empty(result);
    }

    private static List<ValidationResult> Validar(object model)
    {
        var context = new ValidationContext(model);
        var result = new List<ValidationResult>();

        Validator.TryValidateObject(model, context, result, validateAllProperties: true);

        return result;
    }
}
