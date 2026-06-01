using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Repository;

public class TituloRepository(SadaDbContext context) : ITituloRepository
{
    private readonly SadaDbContext _context = context;

    public async Task<TituloModelResponse> IncluirTituloAsync(TituloModelRequest model, CancellationToken cancellationToken = default)
    {
        var status = string.IsNullOrWhiteSpace(model.Status)
            ? (char?)null
            : char.ToUpperInvariant(model.Status.Trim()[0]);

        var nextId = await _context.Titulos
            .Select(item => (int?)item.IdTitulo)
            .MaxAsync(cancellationToken) ?? 0;

        var entity = new TituloModel
        {
            IdTitulo = nextId + 1,
            Titulo = model.Titulo.Trim(),
            Descricao = model.Descricao.Trim(),
            Vencimento = model.Vencimento,
            Status = status
        };

        await _context.Titulos.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new TituloModelResponse
        {
            IdTitulo = entity.IdTitulo,
            Titulo = entity.Titulo,
            Descricao = entity.Descricao,
            Vencimento = entity.Vencimento,
            Status = entity.Status,
            Retorno = true
        };
    }

    public async Task<List<TituloModelResponse>> ListarTitulosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Titulos
            .OrderBy(item => item.IdTitulo)
            .Select(item => new TituloModelResponse
            {
                IdTitulo = item.IdTitulo,
                Titulo = item.Titulo,
                Descricao = item.Descricao,
                Vencimento = item.Vencimento,
                Status = item.Status,
                Retorno = true
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TituloModelResponse>> ListarTitulosAsync(string? status, DateTime? vencimento, CancellationToken cancellationToken = default)
    {
        var query = _context.Titulos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            var normalizedStatus = char.ToUpperInvariant(status.Trim()[0]);
            query = query.Where(item => item.Status == normalizedStatus);
        }

        if (vencimento.HasValue)
        {
            var dataVencimento = vencimento.Value.Date;
            query = query.Where(item =>
                item.Vencimento.HasValue &&
                item.Vencimento.Value.Date == dataVencimento);
        }

        return await query
            .OrderBy(item => item.IdTitulo)
            .Select(item => new TituloModelResponse
            {
                IdTitulo = item.IdTitulo,
                Titulo = item.Titulo,
                Descricao = item.Descricao,
                Vencimento = item.Vencimento,
                Status = item.Status,
                Retorno = true
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<TituloModelResponse?> AlterarTituloAsync(
        TituloModelEditExclusao model,
        CancellationToken cancellationToken = default)
    {
        var entity = await _context.Titulos
            .FirstOrDefaultAsync(item => item.IdTitulo == model.IdTitulo, cancellationToken);

        if (entity is null)
        {
            return null;
        }

        //entity.Titulo = model.Titulo.Trim();
        //entity.Descricao = model.Descricao.Trim();
        entity.Vencimento = model.Vencimento;
        entity.Status = string.IsNullOrWhiteSpace(model.Status)
            ? null
            : char.ToUpperInvariant(model.Status.Trim()[0]);

        await _context.SaveChangesAsync(cancellationToken);

        return new TituloModelResponse
        {
            IdTitulo = entity.IdTitulo,
            Titulo = entity.Titulo,
            Descricao = entity.Descricao,
            Vencimento = entity.Vencimento,
            Status = entity.Status,
            Retorno = true
        };
    }

    public async Task<bool> ApagarTituloAsync(TituloModelEditExclusao model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Titulos
            .FirstOrDefaultAsync(item => item.IdTitulo == model.IdTitulo, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _context.Titulos.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
