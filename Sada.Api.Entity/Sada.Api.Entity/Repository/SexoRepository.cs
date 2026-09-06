using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Repository;

public class SexoRepository : ISexoRepository
{
    private readonly SadaDbContext _context;

    public SexoRepository(SadaDbContext context)
    {
        _context = context;
    }

    public async Task<SexoModelResponse> IncluirSexoAsync(SexoModelRequest model, CancellationToken cancellationToken = default)
    {
        var nextId = await _context.Sexos
            .Select(item => (int?)item.IdSexo)
            .MaxAsync(cancellationToken) ?? 0;

        var entity = new SexoModel
        {
            IdSexo = nextId + 1,
            Descricao = model.Descricao?.Trim() ?? string.Empty,
            Sigla = model.Sigla?.Trim().ToUpperInvariant() ?? string.Empty
        };

        await _context.Sexos.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return MapearResponse(entity);
    }

    public async Task<List<SexoModelResponse>> ListarSexosAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sexos
            .AsNoTracking()
            .OrderBy(item => item.IdSexo)
            .Select(item => new SexoModelResponse
            {
                IdSexo = item.IdSexo,
                Descricao = item.Descricao,
                Sigla = item.Sigla
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SexoModelResponse?> ObterSexoPorIdAsync(int idSexo, CancellationToken cancellationToken = default)
    {
        return await _context.Sexos
            .AsNoTracking()
            .Where(item => item.IdSexo == idSexo)
            .Select(item => new SexoModelResponse
            {
                IdSexo = item.IdSexo,
                Descricao = item.Descricao,
                Sigla = item.Sigla
            })
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<SexoModelResponse?> AlterarSexoAsync(SexoModelRequest model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Sexos
            .SingleOrDefaultAsync(item => item.IdSexo == model.IdSexo, cancellationToken);

        if (entity is null)
            return null;
        
        entity.Descricao = model.Descricao?.Trim() ?? string.Empty;
        entity.Sigla = model.Sigla?.Trim().ToUpperInvariant() ?? string.Empty;

        await _context.SaveChangesAsync(cancellationToken);

        return MapearResponse(entity);
    }

    public async Task<bool> ApagarSexoAsync(int idSexo, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Sexos
            .SingleOrDefaultAsync(item => item.IdSexo == idSexo, cancellationToken);

        if (entity is null)
            return false;  
        
        _context.Sexos.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static SexoModelResponse MapearResponse(SexoModel entity)
    {
        return new SexoModelResponse
        {
            IdSexo = entity.IdSexo,
            Descricao = entity.Descricao,
            Sigla = entity.Sigla
        };
    }
}
