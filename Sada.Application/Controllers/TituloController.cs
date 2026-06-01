using Microsoft.AspNetCore.Mvc;
using Sada.Api.Entity.Model.Response;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Business.Interface;
using System.Runtime.CompilerServices;
using System.Globalization;

namespace Sada.Application.Controllers
{
    [ApiController]
    [Route("titulo")]
    public class TituloController : Controller
    {
        private readonly ITitulo _tituloBusiness;
        private readonly ILogger<TituloController> _logger;

        public TituloController(ILogger<TituloController> logger, ITitulo tituloBusiness)
        {
            _logger = logger;
            _tituloBusiness = tituloBusiness;
        }

        [HttpPost("cadastrar")]
        [ProducesResponseType(typeof(TituloModelResponse), 200)]
        public async Task<IActionResult> CadastrarTitulo([FromBody] TituloModelRequest model)
        {
            if (!await VerificarDadosTitulo(model))
            {
                _logger.LogCritical($"CadastrarTitulo() - Dados do Título são inválidos.");
                return BadRequest("Dados do título inválidos.");
            }

            var cadastrarTitulo = await _tituloBusiness.CadastrarTitulo(model);

            _logger.LogInformation($"Titulo {model.Titulo} cadastrado com sucesso. ");
            return Created();
        }

        [HttpGet("listar-todos-titulos")]
        public async Task<IActionResult> ListarTitulos()
        {
            var listAll = await _tituloBusiness.ListTitulos();
            _logger.LogInformation($"ListarTitulos() - Listagem de Título efetuada com sucesso.");
            return Ok(listAll);
        }

        [HttpGet("listar-titutlos-status-vencimento")]
        public async Task<IActionResult> ListarTitulosByStatusVencimento([FromBody] TituloModelEditExclusao model)
        {
            if (!await VerificarDadosTitulo(model))
            {
                _logger.LogCritical($"ListarTitulosByStatusVencimento() - Dados do Título são inválidos.");
                return BadRequest("ListarTitulosByStatusVencimento() - Dados do Título são inválidos.");
            }
            
            var listByStatusVencimento = await _tituloBusiness.ListTitulos(model.Status, model.Vencimento);
            return Ok(listByStatusVencimento);
        }

        [HttpPut("alterar-titulo")]
        public async Task<IActionResult> AlterarTitulo([FromBody] TituloModelEditExclusao model)
        {
            if (!await VerificarDadosTitulo(model))
            {
                _logger.LogCritical($"AlterarTitulo() - Dados do Título são inválidos.");
                return BadRequest("AlterarTitulo() - Dados do Título são inválidos.");
            }

            var alterarTitulo = await _tituloBusiness.AlterarTitulo(model);

            if (alterarTitulo == null)
            {
                _logger.LogWarning($"AlterarTitulo() - Dados de Título não encontrados.");
                return NotFound(alterarTitulo);
            }

            _logger.LogInformation($"AlterarTitulo() - Alterado com sucessoTítulo.");
            return Ok(alterarTitulo);
        }

        [HttpDelete("apagar-titulo")]
        public async Task<IActionResult> ApagarTitulo([FromBody] TituloModelEditExclusao model)
        {
            if (!await VerificarDadosExclusao(model))
            {
                _logger.LogCritical($"ApagarTitulo() - Dados do Título são inválidos.");
                return BadRequest("ApagarTitulo() - Dados do Título são inválidos.");
            }

            var apagouTitulo = await _tituloBusiness.ApagarTitulo(model);
            if (!apagouTitulo)
            {
                _logger.LogWarning($"ApagarTitulo() - Registro não encontrado.");
                return NotFound($"ApagarTitulo() - Registro não encontrado.");
            }

            _logger.LogInformation($"ApagarTitulo() - Título com ID {model.IdTitulo} apagado com sucesso.");
            return Ok(apagouTitulo);
        }

        private async Task<bool> VerificarDadosTitulo(TituloModelRequest model)
        {
            if (string.IsNullOrEmpty(model.Descricao))
                return await Task.FromResult(false);
            
            if ((model.Status == null) || (model.Status != "P" && model.Status != "C"))
                return await Task.FromResult(false);

            return await Task.FromResult(true);
        }

        private async Task<bool> VerificarDadosTitulo(TituloModelEditExclusao model)
        {
            if ((model.Status == null) || (model.Status != "P" && model.Status != "C"))
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Status inválida.");
                return await Task.FromResult(false);
            }

            if (!model.Vencimento.HasValue)
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Data inválida.");
                return await Task.FromResult(false);
            }

            string data = model.Vencimento.Value.ToString("dd/MM/yyyy");

            bool valida = DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime dataConvertida);

            if (!valida)
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Data inválida.");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        private async Task<bool> VerificarDadosExclusao(TituloModelEditExclusao model)
        {
            if ((model.Status == null) || (model.Status != "P" && model.Status != "C"))
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Status inválida.");
                return await Task.FromResult(false);
            }

            if (!model.Vencimento.HasValue)
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Data inválida.");
                return await Task.FromResult(false);
            }

            string data = model.Vencimento.Value.ToString("dd/MM/yyyy");

            bool valida = DateTime.TryParseExact(data, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime dataConvertida);

            if (!valida)
            {
                _logger.LogCritical($"VerificarDadosTitulo() - Data inválida.");
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }

        [NonAction]
        public IActionResult Index()
        {
            return View();
        }
    }
}
