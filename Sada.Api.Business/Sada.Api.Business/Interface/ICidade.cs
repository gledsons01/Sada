using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Business.Interface
{
    public interface ICidade
    {
        Task<List<CidadeModelResponse>> ListarCidadesAsync();
        Task<List<CidadeModelResponse>> ObterCidadePorIdAsync(int idCidade);
        Task<List<CidadeModelResponse>> ObterCidadesPorUfAsync(int idUf);
        Task<CidadeModelResponse> IncluirCidadeAsync(CidadeModelRequest model);
        Task<CidadeModelResponse> AlterarCidadeAsync(CidadeModelRequest model);
        Task<bool> ApagarCidadeAsync(int idCidade);
    }
}
