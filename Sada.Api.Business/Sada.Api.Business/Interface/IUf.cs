using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business.Interface
{
    public interface IUf
    {
        Task<List<UfModelResponse>> ListUfsAsync();
        Task<List<UfModelResponse>> ObterUfPorIdAsync(int idUf);
        Task<UfModelResponse> IncluirUfAsync(UfModelResponse model);
        Task<UfModelResponse> AlterarUfAsync(UfModelResponse model);
        Task<bool> ApagarUfAsync(int idUf);
    }
}
