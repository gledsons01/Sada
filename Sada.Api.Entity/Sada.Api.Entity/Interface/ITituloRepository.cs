using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Interface;

public interface ITituloRepository
{
    Task<TituloModelResponse> IncluirTituloAsync(TituloModelRequest model, CancellationToken cancellationToken = default);
    Task<List<TituloModelResponse>> ListarTitulosAsync(CancellationToken cancellationToken = default);
    Task<List<TituloModelResponse>> ListarTitulosAsync(string? status, DateTime? vencimento, CancellationToken cancellationToken = default);
    Task<bool> ApagarTituloAsync(TituloModelEditExclusao model, CancellationToken cancellationToken = default);
    Task<TituloModelResponse?> AlterarTituloAsync(TituloModelEditExclusao model, CancellationToken cancellationToken = default);
}
