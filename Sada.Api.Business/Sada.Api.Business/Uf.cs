using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business
{
    public class Uf : IUf
    {
        #region ++ Atributos Globais ++

        private readonly ILogger<Uf> _logger;
        private readonly IUfRepository _ufRepository;

        #endregion ++ Atributos Globais ++

        #region ++ Construtor ++

        public Uf(ILogger<Uf> logger, IUfRepository ufRepository)
        {
            _logger = logger;
            _ufRepository = ufRepository;
        }

        #endregion ++ Construtor ++

        public async Task<List<UfModelResponse>> ListarUfsAsync()
        {
            try
            {
                var ufs = await _ufRepository.ListarUfsAsync();

                return ufs
                    .Select(uf => new UfModelResponse
                    {
                        IdUf = uf.IdUf,
                        SiglaUf = uf.SiglaUf,
                        Sigla = uf.Sigla
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar UFs");
                throw;
            }
        }

        public async Task<List<UfModelResponse>> ObterUfPorIdAsync(int idUf)
        {
            try
            {
                var uf = await _ufRepository.ObterUfPorIdAsync(idUf);

                if (uf == null)
                    throw new InvalidOperationException("UF n�o encontrada");

                return new List<UfModelResponse>
                {
                    new UfModelResponse
                    {
                        IdUf = uf.IdUf,
                        SiglaUf = uf.SiglaUf,
                        Sigla = uf.Sigla
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar UFs");
                throw;
            }
        }

        public async Task<UfModelResponse> IncluirUfAsync(UfModelRequest model)
        {
            try
            {
                var uf = new UfModel
                {
                    IdUf = model.IdUf,
                    SiglaUf = model.SiglaUf,
                    Sigla = model.Sigla
                };
                var result = await _ufRepository.IncluirUfAsync(uf);
                return new UfModelResponse
                {
                    IdUf = result.IdUf,
                    SiglaUf = result.SiglaUf,
                    Sigla = result.Sigla
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir UF");
                throw;
            }
        }

        public async Task<UfModelResponse> AlterarUfAsync(UfModelRequest model)
        {
            try
            {
                var uf = new UfModel
                {
                    IdUf = model.IdUf,
                    SiglaUf = model.SiglaUf,
                    Sigla = model.Sigla
                };
                var result = await _ufRepository.AlterarUfAsync(uf);

                if (result is null)
                    throw new KeyNotFoundException($"UF com ID {model.IdUf} não encontrada.");
                return new UfModelResponse
                {
                    IdUf = result.IdUf,
                    SiglaUf = result.SiglaUf,
                    Sigla = result.Sigla
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar UF");
                throw;
            }
        }

        public async Task<bool> ApagarUfAsync(int idUf)
        {
            try
            {
                var result = await _ufRepository.ApagarUfAsync(idUf);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao apagar UF");
                throw;
            }
        }
    }
}
