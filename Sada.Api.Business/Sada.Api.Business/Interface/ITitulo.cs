using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Business.Interface
{
    public interface ITitulo
    {
        public Task<List<TituloModelResponse>> ListTitulosAsync();
        public Task<List<TituloModelResponse>> ListTitulosAsync(string? status, DateTime? vencimento);
        public Task<TituloModelResponse?> ObterTituloPorIdAsync(int idTitulo);
        public Task<TituloModelResponse> CadastrarTituloAsync(TituloModelRequest model);
        public Task<TituloModelResponse> AlterarTituloAsync(TituloModelEditExclusao model);
        public Task<bool> ApagarTituloAsync(TituloModelEditExclusao model);
    }
}
