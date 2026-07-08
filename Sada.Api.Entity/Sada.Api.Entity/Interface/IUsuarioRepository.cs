using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Interface;

public interface IUsuarioRepository
{
    Task<UsuarioModelResponse> IncluirUsuarioAsync(UsuarioModelRequest model, CancellationToken cancellationToken = default);
    Task<List<UsuarioModelResponse>> ListarUsuariosAsync(CancellationToken cancellationToken = default);
    Task<UsuarioModelResponse?> AlterarUsuarioAsync(UsuarioModelRequest model, CancellationToken cancellationToken = default);
    Task<bool> ApagarUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default);
    Task<UsuarioModelResponse?> ObterUsuarioPorIdAsync(int idUsuario, CancellationToken cancellationToken = default);
    Task<UsuarioModelResponse> LoginUsuario(LoginModelRequest model, CancellationToken cancellationToken = default);
}
