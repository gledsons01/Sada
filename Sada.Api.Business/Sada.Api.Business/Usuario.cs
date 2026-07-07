using Microsoft.Extensions.Logging;
using Sada.Api.Business.Interface;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sada.Api.Business
{
    public class Usuario :IUsuario
    {
        private readonly ILogger<Usuario> _logger;
        private readonly IUsuarioRepository _usuarioRepository;

        public Usuario(ILogger<Usuario> logger, IUsuarioRepository usuarioRepository)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<List<UsuarioModelResponse>> ListUsuarios()
        {
            var listAll = await _usuarioRepository.ListarUsuariosAsync();
            _logger.LogInformation($"Listagem de Registros Cadastrados -  {listAll.Count}. ");
            return listAll;
        }

        public async Task<UsuarioModelResponse> CadastrarUsuario(UsuarioModelRequest model)
        {
            try
            {
                var result = await _usuarioRepository.IncluirUsuarioAsync(model);
                _logger.LogInformation($"Cadastro de Usuario {model.NomeUsuario} efetuado com sucesso.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao cadastrar usuario {model.NomeUsuario}");
                throw;
            }
        }

        public async Task<UsuarioModelResponse?> AlterarUsuario(UsuarioModelRequest model)
        {
            var result = await _usuarioRepository.AlterarUsuarioAsync(model);
            if (result is null)
            {
                _logger.LogWarning($"Usuario não encontrado para alteração. ID: {model.IdUsuario}");
                throw new KeyNotFoundException($"Usuario com ID {model.IdUsuario} não encontrado.");
            }
            _logger.LogInformation($"Alteração de Usuario {model.NomeUsuario} efetuada com sucesso.");
            return result;
        }

        public async Task<bool> ApagarUsuario(int idUsuario)
        {
            var result = await _usuarioRepository.ApagarUsuarioAsync(idUsuario);
            if (!result)
            {
                _logger.LogWarning($"Usuario não encontrado para exclusão. ID: {idUsuario}");
                throw new KeyNotFoundException($"Usuario com ID {idUsuario} não encontrado.");
            }
            _logger.LogInformation($"Exclusão de Usuario ID {idUsuario} efetuada com sucesso.");
            return result;
        }

        public async Task<UsuarioModelResponse?> ObterUsuarioPorId(int idUsuario)
        {
            var result = await _usuarioRepository.ObterUsuarioPorIdAsync(idUsuario);
            if (result is null)
            {
                _logger.LogWarning($"Usuario não encontrado. ID: {idUsuario}");
                throw new KeyNotFoundException($"Usuario com ID {idUsuario} não encontrado.");
            }
            _logger.LogInformation($"Consulta de Usuario ID {idUsuario} efetuada com sucesso.");
            return result;
        }

        public async Task<UsuarioModelResponse> LoginUsuario(LoginModelRequest model)
        {
            var result = await _usuarioRepository.LoginUsuario(model);
            if (result is null)
            {
                _logger.LogWarning($"Login falhou para o usuário: {model.Login}");
                throw new UnauthorizedAccessException($"Login falhou para o usuário: {model.Login}");
            }
            _logger.LogInformation($"Login bem-sucedido para o usuário: {model.Login}");
            return result;
        }
    }
}
