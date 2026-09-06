using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Interface
{
    public interface ISexoRepository
    {
        Task<SexoModelResponse> IncluirSexoAsync(SexoModelRequest model, CancellationToken cancellationToken = default);
        Task<List<SexoModelResponse>> ListarSexosAsync(CancellationToken cancellationToken = default);
        Task<SexoModelResponse?> ObterSexoPorIdAsync(int idSexo, CancellationToken cancellationToken = default);
        Task<SexoModelResponse?> AlterarSexoAsync(SexoModelRequest model, CancellationToken cancellationToken = default);
        Task<bool> ApagarSexoAsync(int idSexo, CancellationToken cancellationToken = default);

    }
}
