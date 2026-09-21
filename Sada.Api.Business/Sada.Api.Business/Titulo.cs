using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Business
{
    public class Titulo : ITitulo
    {
        #region ++ Atributos Globais ++

        private readonly ILogger<Titulo> _logger;
        private readonly ITituloRepository _tituloRepository;

        #endregion ++ Atributos Globais ++

        #region ++ Construtor ++

        public Titulo(ILogger<Titulo> logger, ITituloRepository tituloRepository)
        {
            _logger = logger;
            _tituloRepository = tituloRepository;
        }

        #endregion ++ Construtor ++

        public async Task<List<TituloModelResponse>> ListTitulosAsync()
        {
            var listAll = await _tituloRepository.ListarTitulosAsync();
            _logger.LogInformation($"Listagem de Registros Cadastrados -  {listAll.Count}. " );
            return listAll;
        }

        public async Task<List<TituloModelResponse>> ListTitulosAsync(string? status, DateTime? vencimento)
        {
            var listFiltered = await _tituloRepository.ListarTitulosAsync(status, vencimento);
            _logger.LogInformation($"Listagem de Registros Filtrados - {listFiltered.Count} .");
            return listFiltered;
        }

        public async Task<TituloModelResponse?> ObterTituloPorIdAsync(int idTitulo)
        {
            var result = await _tituloRepository.ObterTituloPorIdAsync(idTitulo);
            if (result is null)
            {
                _logger.LogWarning($"Titulo não encontrado. ID: {idTitulo}");
                throw new KeyNotFoundException($"Titulo com ID {idTitulo} não encontrado.");
            }
            _logger.LogInformation($"Titulo obtido. ID: {idTitulo}");
            return result;
        }

        public async Task<TituloModelResponse> CadastrarTituloAsync(TituloModelRequest model)
        {
            try
            {
                var result = await _tituloRepository.IncluirTituloAsync(model);
                _logger.LogInformation($"Cadastro de Titulo {model.Titulo} efetuado com sucesso.");
                return result;                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao cadastrar titulo {model.Titulo}");
                throw;
            }
        }

        public async Task<TituloModelResponse> AlterarTituloAsync(TituloModelEditExclusao model)
        {
            var result = await _tituloRepository.AlterarTituloAsync(model);

            if (result is null)
            {
                _logger.LogWarning($"Titulo não encontrado para alteração. ID: {model.IdTitulo}");
                throw new KeyNotFoundException($"Titulo com ID {model.IdTitulo} não encontrado.");
            }

            _logger.LogInformation($"Titulo alterado. ID: {model.IdTitulo}");
            return result;
        }

        public async Task<bool> ApagarTituloAsync(TituloModelEditExclusao model)
        {
            var result = await _tituloRepository.ApagarTituloAsync(model);

            _logger.LogInformation($"Titulo apagado. ID: {model.IdTitulo}");
            return result;
        }
    }
}
