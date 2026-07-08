using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;
using System.ComponentModel;

namespace Sada.Api.Entity.Repository;

public class UsuarioRepository(SadaDbContext context, ILogger<UsuarioRepository> logger) : IUsuarioRepository
{
    private readonly SadaDbContext _context = context;
    private readonly ILogger<UsuarioRepository> _logger = logger;
    
    public async Task<UsuarioModelResponse> IncluirUsuarioAsync(UsuarioModelRequest model, CancellationToken cancellationToken = default)
    {
        var nextId = await _context.Usuarios
            .Select(item => (int?)item.IdUsuario)
            .MaxAsync(cancellationToken) ?? 0;

        var entity = new UsuarioModel
        {
            IdUsuario = nextId + 1,
            NomeUsuario = model.NomeUsuario.Trim(),
            Login = model.Login.Trim(),
            Senha = model.Senha.Trim(),
            Endereco = model.Endereco.Trim(),
            NumeroEndereco = model.NumeroEndereco.Trim(),
            Bairro = model.Bairro.Trim(),
            IdUf = model.IdUf ?? 0,
            IdCidade = model.IdCidade ?? 0,
            NomeSocial = model.NomeSocial.Trim(),
            ID_SEXO = model.IdSexo,
            E_MAIL  = model.EMail.Trim()
        };

        await _context.Usuarios.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new UsuarioModelResponse
        {
            IdUsuario = entity.IdUsuario,
            blnRetorno = true
        };
    }

    public async Task<List<UsuarioModelResponse>> ListarUsuariosAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Usuarios
                .OrderBy(item => item.IdUsuario)
                .Select(item => new UsuarioModelResponse
                {
                    IdUsuario = item.IdUsuario,
                    NomeUsuario = item.NomeUsuario,
                    Login = item.Login,
                    Senha = item.Senha,
                    Endereco = item.Endereco,
                    NumeroEndereco = item.NumeroEndereco,
                    Bairro = item.Bairro,
                    IdUf = item.IdUf,
                    IdCidade = item.IdCidade,
                    NomeSocial = item.NomeSocial,
                    IdSexo = item.ID_SEXO,
                    EMail = item.E_MAIL,
                    blnRetorno = true
                })
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao listar usuarios.");
            throw;
        }
    }

    public async Task<UsuarioModelResponse?> AlterarUsuarioAsync(UsuarioModelRequest model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Usuarios
            .FirstOrDefaultAsync(item => item.IdUsuario == model.IdUsuario, cancellationToken);

        if (entity == null)
        {
            return null;
        }
        entity.NomeUsuario = model.NomeUsuario.Trim();
        entity.Login = model.Login.Trim();
        entity.Senha = model.Senha.Trim();
        entity.Endereco = model.Endereco.Trim();
        entity.NumeroEndereco = model.NumeroEndereco.Trim();
        entity.Bairro = model.Bairro.Trim();
        entity.IdUf = model.IdUf ?? 0;
        entity.IdCidade = model.IdCidade ?? 0;
        entity.NomeSocial = model.NomeSocial.Trim();
        entity.ID_SEXO = model.IdSexo;
        entity.E_MAIL = model.EMail.Trim();
        _context.Usuarios.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new UsuarioModelResponse
        {
            IdUsuario = entity.IdUsuario,
            blnRetorno = true
        };
    }

    public async Task<bool> ApagarUsuarioAsync(int idUsuario, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Usuarios
            .FirstOrDefaultAsync(item => item.IdUsuario == idUsuario, cancellationToken);
        if (entity == null)
        {
            return false;
        }
        _context.Usuarios.Remove(entity);
        
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<UsuarioModelResponse?> ObterUsuarioPorIdAsync(int idUsuario, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Usuarios
            .FirstOrDefaultAsync(item => item.IdUsuario == idUsuario, cancellationToken);
        if (entity == null)
        {
            return null;
        }
        return new UsuarioModelResponse
        {
            IdUsuario = entity.IdUsuario,
            NomeUsuario = entity.NomeUsuario,
            Login = entity.Login,
            Senha = entity.Senha,
            Endereco = entity.Endereco,
            NumeroEndereco = entity.NumeroEndereco,
            Bairro = entity.Bairro,
            IdUf = entity.IdUf,
            IdCidade = entity.IdCidade,
            NomeSocial = entity.NomeSocial,
            IdSexo = entity.ID_SEXO,
            EMail = entity.E_MAIL,
            blnRetorno = true
        };
    }

    public async Task<UsuarioModelResponse> LoginUsuario(LoginModelRequest model, CancellationToken cancellationToken = default)
    {
        var login = model.Login?.Trim();
        var senha = model.Senha?.Trim();

        var entity = await _context.Usuarios
            .FirstOrDefaultAsync(item => item.Senha == senha && item.Login == login, cancellationToken);

        if (entity == null)
        {
            throw new InvalidOperationException("Usuario ou senha invalidos");
        }

        return new UsuarioModelResponse
        {
            IdUsuario = entity.IdUsuario,
            NomeUsuario = entity.NomeUsuario,
            Login = entity.Login,
            EMail = entity.E_MAIL,
            blnRetorno = true
        };
    }
}
