using Sada.Api.Entity.Model.Response;

namespace Sada.Application.Services;

public interface IJwtTokenService
{
    TokenModelResponse PreencherTokens(UsuarioModelResponse usuario);
}
