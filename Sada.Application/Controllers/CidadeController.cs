using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Application.Services;

namespace Sada.Application.Controllers
{
    public class CidadeController : Controller
    {
        private readonly ICidade _cidadeBusiness;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<CidadeController> _logger;

        public CidadeController(ICidade cidadeBusiness, 
            IJwtTokenService jwtTokenService, 
            ILogger<CidadeController> logger)
        {
            _cidadeBusiness = cidadeBusiness;
            _jwtTokenService = jwtTokenService;
            _logger = logger;            
        }

        [HttpGet("listar-cidade")]
        public async Task<IActionResult> ListarCidadeAsync()
        {
            try
            {
                var result = await _cidadeBusiness.ListarCidadesAsync();
                _logger.LogInformation($"Listagem de cidades efetuada com sucesso.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar as cidades.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao de listagem de cidades.");
            }
        }

        [HttpGet("obter-cidade")]
        public async Task<IActionResult> ObterCidadeAsync(int id)
        {
            try
            {
                var result = await _cidadeBusiness.ObterCidadePorIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning($"Cidade não encontrada {id}.");
                    return NotFound($"Cidade não encontrada {id}.");
                }

                _logger.LogInformation($"Cidade localizada {id}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter a cidade.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao de obter cidade.");
            }
        }

        [HttpPost("cadastrar-cidade")]
        public async Task<IActionResult> CadastrarCidadeAsync([FromBody] CidadeModelRequest modelRequest)
        {
            try
            {
                var result = await _cidadeBusiness.IncluirCidadeAsync(modelRequest);
                _logger.LogInformation($"Cadasdo de cidade efetuado com sucesso. {result}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar a cidade.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao de cadastar a cidade.");
            }
        }

        [HttpPatch("alterar-cidade")]
        public async Task<IActionResult> AlterarCidadeAsync([FromBody] CidadeModelRequest modelRequest)
        {
            try
            {
                var result = await _cidadeBusiness.AlterarCidadeAsync(modelRequest);

                if (result == null)
                {
                    _logger.LogWarning($"Alteração de ciade não efetuada. Cidade não encontrada {modelRequest.DescricaoCidade}.");
                    return NotFound($"Alteração de cidade não efetuada. Cidade não encontrada {modelRequest.DescricaoCidade}.");
                }

                _logger.LogInformation($"Alteração de cidade efetuada com sucesso.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar dados da cidade.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao de alterar dados da cidade.");
            }
        }

        [HttpDelete("apagar-cidade")]
        public async Task<IActionResult> ApagarCidadeAsync(int id)
        {
            try
            {
                var result = await _cidadeBusiness.ApagarCidadeAsync(id);

                if (result)
                {
                    _logger.LogWarning($"Erro ao localizar a cidade {id}.");
                    return NotFound($"Exclusão de cidade não efetuada. {id}.");
                }

                _logger.LogInformation($"Exclusão de cidade efetuada com sucesso.");
                return Ok(result);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao excluir dados da cidade {id}.");
                return StatusCode(500, $"Erro ao excluir dados da cidade {id}.");
            }
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
