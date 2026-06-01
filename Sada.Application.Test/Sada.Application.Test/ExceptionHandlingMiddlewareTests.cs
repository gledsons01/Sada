using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Sada.Application.Middlewares;

namespace Sada.Application.Test;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_QuandoNaoHaExcecao_DeveExecutarProximoMiddleware()
    {
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(logger);
        var context = CriarHttpContext();
        var nextCalled = false;

        await middleware.InvokeAsync(context, _ =>
        {
            nextCalled = true;
            return Task.CompletedTask;
        });

        Assert.True(nextCalled);
        Assert.Empty(logger.Entries);
    }

    [Fact]
    public async Task InvokeAsync_ComKeyNotFoundException_DeveRetornarNotFoundComDetalhe()
    {
        await AssertErroMapeadoAsync(
            new KeyNotFoundException("Titulo não encontrado"),
            StatusCodes.Status404NotFound,
            "Titulo não encontrado");
    }

    [Fact]
    public async Task InvokeAsync_ComValidationException_DeveRetornarUnprocessableEntityComDetalhe()
    {
        await AssertErroMapeadoAsync(
            new ValidationException("Campo inválido"),
            StatusCodes.Status422UnprocessableEntity,
            "Campo inválido");
    }

    [Fact]
    public async Task InvokeAsync_ComExcecaoNaoMapeada_DeveRetornarInternalServerErrorGenerico()
    {
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(logger);
        var context = CriarHttpContext();
        var exception = new InvalidOperationException("Erro interno sensível");

        await middleware.InvokeAsync(context, _ => throw exception);

        var json = await LerJsonAsync(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);
        Assert.Equal("Ocorreu um erro na operação", json.RootElement.GetProperty("title").GetString());
        Assert.Equal(StatusCodes.Status500InternalServerError, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal("Tente novamente ou entre em contato com o nosso atendimento", json.RootElement.GetProperty("detail").GetString());

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Same(exception, log.Exception);
    }

    private static async Task AssertErroMapeadoAsync(Exception exception, int statusCode, string detail)
    {
        var logger = new TestLogger<ExceptionHandlingMiddleware>();
        var middleware = new ExceptionHandlingMiddleware(logger);
        var context = CriarHttpContext();

        await middleware.InvokeAsync(context, _ => throw exception);

        var json = await LerJsonAsync(context);
        Assert.Equal(statusCode, context.Response.StatusCode);
        Assert.StartsWith("application/json", context.Response.ContentType);
        Assert.Equal(statusCode, json.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(detail, json.RootElement.GetProperty("detail").GetString());

        var log = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Error, log.Level);
        Assert.Same(exception, log.Exception);
    }

    private static DefaultHttpContext CriarHttpContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static async Task<JsonDocument> LerJsonAsync(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return await JsonDocument.ParseAsync(context.Response.Body);
    }
}
