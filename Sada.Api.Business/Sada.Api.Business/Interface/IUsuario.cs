using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Interface
{
    public interface IUsuario
    {
        Task<List<UsuarioModelResponse>> ListUsuariosAsync();
        Task<UsuarioModelResponse> CadastrarUsuarioAsync(UsuarioModelRequest model);
        Task<UsuarioModelResponse?> AlterarUsuarioAsync(UsuarioModelRequest model);
        Task<bool> ApagarUsuarioAsync(int idUsuario);
        Task<UsuarioModelResponse?> ObterUsuarioPorIdAsync(int idUsuario);
        Task<UsuarioModelResponse> LoginUsuarioAsync(LoginModelRequest model);        
    }
}
