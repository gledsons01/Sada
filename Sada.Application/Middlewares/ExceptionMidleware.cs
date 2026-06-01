using System.ComponentModel.DataAnnotations;

namespace Sada.Application.Middlewares
{
    internal sealed class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An exception occurred: {Message}", e.Message);
                await HandleExceptionAsync(context, e);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            var statusCode = GetStatusCode(exception);

            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            switch (statusCode)
            {
                case StatusCodes.Status404NotFound:
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        //title = GetsTitle(exception),
                        status = statusCode,
                        detail = exception.Message
                    });
                    break;
                case StatusCodes.Status422UnprocessableEntity:
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        //title = GetsTitle(exception),
                        status = statusCode,
                        detail = exception.Message
                    });
                    break;
                case StatusCodes.Status400BadRequest:
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        //title = GetsTitle(exception),
                        status = statusCode,
                        detail = exception.Message
                    });
                    break;
                case StatusCodes.Status401Unauthorized:
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        //title = GetsTitle(exception),
                        status = statusCode,
                        detail = exception.Message
                    });
                    break;
                default:
                    await httpContext.Response.WriteAsJsonAsync(new
                    {
                        title = "Ocorreu um erro na operação",
                        status = statusCode,
                        detail = "Tente novamente ou entre em contato com o nosso atendimento",
                    });
                    break;
            }
        }

        private static int GetStatusCode(Exception exception) =>
            exception switch
            {
                KeyNotFoundException => StatusCodes.Status404NotFound,
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                _ => StatusCodes.Status500InternalServerError
            };
    }

     //private static int GetStatusCode(Exception exception) =>
     //   exception switch
     //   {
     //       UnauthorizedLoginException => StatusCodes.Status401Unauthorized,
     //       KeycloakBadException => StatusCodes.Status400BadRequest,
     //       ProjectNotFoundException => StatusCodes.Status404NotFound,
     //       ValidationException => StatusCodes.Status422UnprocessableEntity,
     //       _ => StatusCodes.Status500InternalServerError
     //   };

     //   private static string GetsTitle(Exception exception) =>
     //       exception switch
     //       {
     //           UnauthorizedLoginException => "Sem Autorização!",
     //           KeycloakBadException => "Ocorreu um erro na operação",
     //           ProjectNotFoundException => "Recurso não encontrado",
     //           ValidationException => "Erro de Validação",
     //           _ => "Server Error"
     //       };
    }
