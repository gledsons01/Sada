using Sada.Api.Entity.Model.Response;

namespace Sada.Application.Services;

public interface IJwtTokenService
{
    void PreencherTokens(UsuarioModelResponse usuario);
}
