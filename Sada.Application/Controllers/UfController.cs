using Microsoft.AspNetCore.Mvc;
using Sada.Api.Entity.Model.Response;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Business.Interface;
using Sada.Application.Services;

namespace Sada.Application.Controllers
{
    [ApiController]
    [Route("uf")]
    public class UfController : Controller
    {
        private readonly IUf _ufBusiness;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<UfController> _logger;

        public UfController(IUf ufBusiness, IJwtTokenService jwtTokenService, ILogger<UfController> logger)
        {
            _ufBusiness = ufBusiness;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        [HttpGet("listar-uf")]
        public async Task<IActionResult> ListarUfAsync()
        {
            try
            {
                var result = await _ufBusiness.ListarUfsAsync();
                _logger.LogInformation($"Listagem de UFs efetuada com sucesso.");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar as UFs.");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao de listagem de UFs.");
            }
        }

        [HttpGet("obter-uf/{id}")]
        public async Task<IActionResult> ObterUfAsync(int id)
        {
            try
            {
                var result = await _ufBusiness.ObterUfPorIdAsync(id);

                if (result == null)
                {
                    _logger.LogWarning($"UF não encontrada ID: {id}");
                    return NotFound($"UF com ID {id} não encontrada.");
                }

                _logger.LogInformation($"UF encontrada com sucesso através do Id: {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter UF com ID {id}");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPost("cadastrar-uf")]
        public async Task<IActionResult> CadastrarUfAsync([FromBody] UfModelRequest ufModelRequest)
        {
            try
            {
                var result = await _ufBusiness.IncluirUfAsync(ufModelRequest);
                _logger.LogInformation($"UF cadastrada com sucesso: {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar UF");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPatch("alterar-uf/{id}")]
        public async Task<IActionResult> AlterarUfAsync(int id, [FromBody] UfModelRequest ufModelRequest)
        {
            try
            {
                ufModelRequest.IdUf = id;
                var result = await _ufBusiness.AlterarUfAsync(ufModelRequest);
                _logger.LogInformation($"UF alterada com sucesso: {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar UF");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpDelete("apagar-uf/{id}")]
        public async Task<IActionResult> ApagarUfAsync(int id)
        {
            try
            {
                var result = await _ufBusiness.ApagarUfAsync(id);
                _logger.LogInformation($"UF apagada com sucesso: {result}");
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao apagar UF");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [NonAction]
        public IActionResult Index()
        {
            return View();
        }
    }
}
