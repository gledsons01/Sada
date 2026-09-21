using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Interface
{
    public interface ICidadeRepository
    {
        Task<CidadeModel> IncluirCidadeAsync(CidadeModelRequest model, CancellationToken cancellationToken = default);
        Task<List<CidadeModel>> ListarCidadesAsync(CancellationToken cancellationToken = default);
        Task<List<CidadeModel>> ListarCidadesPorUfAsync(int idUf, CancellationToken cancellationToken = default);
        Task<CidadeModel?> ObterCidadePorIdAsync(int idCidade, CancellationToken cancellationToken = default);
        Task<CidadeModel?> AlterarCidadeAsync(CidadeModelRequest model, CancellationToken cancellationToken = default);
        Task<bool> ApagarCidadeAsync(int idCidade, CancellationToken cancellationToken = default);

    }
}
