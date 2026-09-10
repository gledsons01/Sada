using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Interface
{
    public interface IUf
    {
        Task<List<UfModelResponse>> ListarUfsAsync();
        Task<List<UfModelResponse>> ObterUfPorIdAsync(int idUf);
        Task<UfModelResponse> IncluirUfAsync(UfModelRequest model);
        Task<UfModelResponse> AlterarUfAsync(UfModelRequest model);
        Task<bool> ApagarUfAsync(int idUf);
    }
}
