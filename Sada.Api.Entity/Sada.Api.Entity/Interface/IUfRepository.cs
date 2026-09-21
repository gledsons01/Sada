using Sada.Api.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Entity.Interface
{
    public interface IUfRepository
    {
        Task<UfModel> IncluirUfAsync(UfModel model, CancellationToken cancellationToken = default);
        Task<List<UfModel>> ListarUfsAsync(CancellationToken cancellationToken = default);
        Task<UfModel?> ObterUfPorIdAsync(int idUf, CancellationToken cancellationToken = default);
        Task<UfModel?> AlterarUfAsync(UfModel model, CancellationToken cancellationToken = default);
        Task<bool> ApagarUfAsync(int idUf, CancellationToken cancellationToken = default);

    }
}
