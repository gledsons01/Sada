using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business
{
    public class Cidade : ICidade
    {
        #region ++ Atributos Globais ++

        private readonly ILogger<Cidade> _logger;
        private readonly ICidadeRepository _cidadeRepository;

        #endregion ++ Atributos Globais ++

        #region ++ Construtor ++

        public Cidade(ILogger<Cidade> logger, ICidadeRepository cidadeRepository)
        {
            _logger = logger;
            _cidadeRepository = cidadeRepository;
        }

        #endregion ++ Construtor ++

        public async Task<List<CidadeModelResponse>> ListarCidadesAsync()
        {
            try
            {
                var cidades = await _cidadeRepository.ListarCidadesAsync();
                return cidades.Select(c => new CidadeModelResponse
                {
                    IdCidade = c.IdCidade,
                    DescricaoCidade = c.DescricaoCidade,
                    IdUf = c.IdUf
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar cidades");
                throw;
            }
        }

        public async Task<List<CidadeModelResponse>> ObterCidadePorIdAsync(int idCidade)
        {
            try
            {
                var cidade = await _cidadeRepository.ObterCidadePorIdAsync(idCidade);

                if (cidade == null)
                    throw new InvalidOperationException("Cidade não encontrada");

                return new List<CidadeModelResponse>
                {
                    new CidadeModelResponse
                    {
                        IdCidade = cidade.IdCidade,
                        DescricaoCidade = cidade.DescricaoCidade,
                        IdUf = cidade.IdUf
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter cidade com ID {idCidade}");
                throw;
            }
        }

        public async Task<List<CidadeModelResponse>> ObterCidadesPorUfAsync(int idUf)
        {
            try
            {
                var cidades = await _cidadeRepository.ListarCidadesPorUfAsync(idUf);
                return cidades.Select(c => new CidadeModelResponse
                {
                    IdCidade = c.IdCidade,
                    DescricaoCidade = c.DescricaoCidade,
                    IdUf = c.IdUf
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter cidades para UF com ID {idUf}");
                throw;
            }
        }

        public async Task<CidadeModelResponse> IncluirCidadeAsync(CidadeModelRequest model)
        {
            try
            {
                var result = await _cidadeRepository.IncluirCidadeAsync(model);
                return new CidadeModelResponse
                {
                    IdCidade = result.IdCidade,
                    DescricaoCidade = result.DescricaoCidade,
                    IdUf = result.IdUf
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir cidade");
                throw;
            }
        }

        public async Task<CidadeModelResponse> AlterarCidadeAsync(CidadeModelRequest model)
        {
            try
            {
                var result = await _cidadeRepository.AlterarCidadeAsync(model);
                if (result == null)
                    throw new InvalidOperationException("Cidade não encontrada para alteração");
                return new CidadeModelResponse
                {
                    IdCidade = result.IdCidade,
                    DescricaoCidade = result.DescricaoCidade,
                    IdUf = result.IdUf
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao alterar cidade com ID {model.IdCidade}");
                throw;
            }
        }

        public async Task<bool> ApagarCidadeAsync(int idCidade)
        {
            try
            {
                var result = await _cidadeRepository.ApagarCidadeAsync(idCidade);
                if (!result)
                    throw new InvalidOperationException("Cidade não encontrada para exclusão");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao apagar cidade com ID {idCidade}");
                throw;
            }
        }
    }
}