using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Interface
{
    public interface ISexo
    {
        Task<List<SexoModelResponse>> ListarSexosAsync();
        Task<SexoModelResponse> ObterSexoPorIdAsync(int idSexo);
        Task<SexoModelResponse> CadastrarSexoAsync(SexoModelRequest model);        
        Task<SexoModelResponse> AlterarSexoAsync(SexoModelRequest model);
        Task<bool> ExcluirSexoAsync(int idSexo);
    }
}
