using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Business.Interface
{
    public interface ITitulo
    {
        public Task<List<TituloModelResponse>> ListTitulos();
        public Task<List<TituloModelResponse>> ListTitulos(string? status, DateTime? vencimento);
        public Task<TituloModelResponse> CadastrarTitulo(TituloModelRequest model);
        public Task<TituloModelResponse> AlterarTitulo(TituloModelEditExclusao model);
        public Task<bool> ApagarTitulo(TituloModelEditExclusao model);
    }
}
