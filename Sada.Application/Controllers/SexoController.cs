using Microsoft.AspNetCore.Mvc;
using Sada.Api.Entity.Model.Response;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Business.Interface;
using Sada.Application.Services;
using Microsoft.VisualBasic;

namespace Sada.Application.Controllers
{
    [ApiController]
    [Route("sexo")]
    public class SexoController : Controller
    {
        private readonly ISexo _sexoBusiness;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<SexoController> _logger;

        public SexoController(ILogger<SexoController> logger, ISexo sexoBusiness, IJwtTokenService jwtTokenService)
        {
            _logger = logger;
            _sexoBusiness = sexoBusiness;
            _jwtTokenService = jwtTokenService;            
        }

        [HttpGet("listar-sexo")]
        [ProducesResponseType(typeof(SexoModelResponse), 200)]
        public async Task<IActionResult> ListarSexoAsync()
        {
            try
            {
                var result = await _sexoBusiness.ListarSexosAsync();
                _logger.LogInformation("Listagem de Sexo efetuada com sucesso !!!");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpGet("capturar-sexo/{id}")]
        [ProducesResponseType(typeof(SexoModelResponse), 200)]
        public async Task<IActionResult> CapturarSexoAsync(int id)
        {
            try
            {
                var result = await _sexoBusiness.ObterSexoPorIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning($"Sexo não encontrado ID: {id}");
                    return NotFound($"Usuario com ID {id} nao encontrado.");
                }
                
                _logger.LogInformation($"Tipo de sexo encontrado com sucesso através do Id: {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter tipo de sexo com ID {id}");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPost("cadastrar-sexo")]
        [ProducesResponseType(typeof(SexoModelResponse), 200)]
        public async Task<IActionResult> CadastrarSexoAsync([FromBody] SexoModelRequest modelRequest)
        {
            try
            {
                var result = await _sexoBusiness.CadastrarSexoAsync(modelRequest);
                _logger.LogInformation($"Cadasdo do Tipo de Sexo efetuado com sucesso. {result}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar tipo de sexo.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPatch("alterar-sexo")]
        public async Task<IActionResult> AlterSexoAsync([FromBody] SexoModelRequest modelRequest)
        {
            try
            {
                var result = await _sexoBusiness.AlterarSexoAsync(modelRequest);

                if (result == null)
                {
                    _logger.LogWarning($"Sexo não encontrado. {modelRequest.Sigla}.");
                    return NotFound($"Sexo não encontrado. {modelRequest.Sigla}.");
                }

                _logger.LogInformation($"Sexo alterado com sucesso. {modelRequest.Sigla}.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar tipo de sexo.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpDelete("deletar-sexo")]
        public async Task<IActionResult> ApagarSexoAsync(int id)
        {
            try
            {
                var result = await _sexoBusiness.ExcluirSexoAsync(id);

                if (!result)
                {
                    _logger.LogWarning($"Tipo de sexo não encontrado. {id.ToString()}.");
                    return NotFound($"Tipo de Sexo não encontrado. {id}.");
                }

                _logger.LogInformation($"Tipo de Sexo excluído com suscesso. {id}.");
                return Ok(result);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao apagar sexo tipo de sexo.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }
    }
}
