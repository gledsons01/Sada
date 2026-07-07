using Microsoft.AspNetCore.Mvc;
using Sada.Api.Entity.Model.Response;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Business.Interface;
using Sada.Application.Services;

namespace Sada.Application.Controllers
{
    [ApiController]
    [Route("usuario")]
    public class UsuarioController : Controller
    {
        private readonly IUsuario _usuarioBusiness;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ILogger<UsuarioController> logger, IUsuario usuarioBusiness, IJwtTokenService jwtTokenService)
        {
            _logger = logger;
            _usuarioBusiness = usuarioBusiness;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("cadastrar")]
        [ProducesResponseType(typeof(UsuarioModelResponse), 200)]
        public async Task<IActionResult> CadastrarUsuario([FromBody] UsuarioModelRequest usuarioModelRequest)
        {
            try
            {
                var result = await _usuarioBusiness.CadastrarUsuario(usuarioModelRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuario");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpGet("listar-usuarios")]
        public async Task<IActionResult> ListarUsuarios()
        {
            try
            {
                var result = await _usuarioBusiness.ListUsuarios();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar usuarios");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpGet("listar-usuario/{id}")]
        public async Task<IActionResult> ObterUsuarioPorId(int id)
        {
            try
            {
                var result = await _usuarioBusiness.ObterUsuarioPorId(id);
                if (result == null)
                {
                    return NotFound($"Usuario com ID {id} nao encontrado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter usuario com ID {id}");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPut("alterar-usuario")]
        public async Task<IActionResult> AlterarUsuario([FromBody] UsuarioModelRequest usuarioModelRequest)
        {
            try
            {
                var result = await _usuarioBusiness.AlterarUsuario(usuarioModelRequest);
                if (result == null)
                {
                    return NotFound($"Usuario com ID {usuarioModelRequest.IdUsuario} nao encontrado.");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao alterar usuario com ID {usuarioModelRequest.IdUsuario}");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpDelete("apagar-usuario/{id}")]
        public async Task<IActionResult> ApagarUsuario(int id)
        {
            try
            {
                var result = await _usuarioBusiness.ApagarUsuario(id);
                if (!result)
                {
                    return NotFound($"Usuario com ID {id} nao encontrado.");
                }
                return Ok($"Usuario com ID {id} apagado com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao apagar usuario com ID {id}");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        [HttpPost("login-usuario")]
        public async Task<IActionResult> LoginUsuario([FromQuery] string email, [FromQuery] string senha)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                    return BadRequest("LoginUsuario() - Email e senha sao obrigatorios.");

                LoginModelRequest model = new LoginModelRequest();
                model.Login = email;
                model.Senha = senha;

                var result = await _usuarioBusiness.LoginUsuario(model);
                if (result == null)
                {
                    return NotFound("Usuario nao encontrado.");
                }

                _jwtTokenService.PreencherTokens(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer login do usuario");
                return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
            }
        }

        //[HttpPost("auth")]
        //public async Task<IActionResult> Authenticate([FromBody] UsuarioModelRequest usuarioModelRequest)
        //{
        //    try
        //    {
        //        var result = await _usuarioBusiness.LoginUsuario(usuarioModelRequest);
        //        if (result == null)
        //        {
        //            return Unauthorized("Usuario ou senha invalidos.");
        //        }
        //        _jwtTokenService.PreencherTokens(result);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Erro ao autenticar usuario");
        //        return StatusCode(500, "Ocorreu um erro ao processar a solicitacao.");
        //    }
        //}
    }
}
