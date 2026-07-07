using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Business.Interface
{
    public interface IUsuario
    {
        Task<List<UsuarioModelResponse>> ListUsuarios();
        Task<UsuarioModelResponse> CadastrarUsuario(UsuarioModelRequest model);
        Task<UsuarioModelResponse?> AlterarUsuario(UsuarioModelRequest model);
        Task<bool> ApagarUsuario(int idUsuario);
        Task<UsuarioModelResponse?> ObterUsuarioPorId(int idUsuario);
        Task<UsuarioModelResponse> LoginUsuario(LoginModelRequest model);        
    }
}
