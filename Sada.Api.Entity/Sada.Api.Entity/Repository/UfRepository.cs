using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;
using Sada.Api.Entity.Model.Response;

namespace Sada.Api.Entity.Repository;

public class UfRepository(SadaDbContext context) : IUfRepository
{
    private readonly SadaDbContext _context = context;

    public async Task<UfModel> IncluirUfAsync(UfModel model, CancellationToken cancellationToken = default)
    {
        var nextId = await _context.Ufs
            .Select(item => (int?)item.IdUf)
            .MaxAsync(cancellationToken) ?? 0;

        var entity = new UfModel
        {
            IdUf = nextId + 1,
            SiglaUf = NormalizarSigla(model.SiglaUf),
            Sigla = model.Sigla?.Trim()
        };

        await _context.Ufs.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<UfModel>> ListarUfsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Ufs
            .AsNoTracking()
            .OrderBy(item => item.IdUf)
            .ToListAsync(cancellationToken);
    }

    public async Task<UfModel?> ObterUfPorIdAsync(int idUf, CancellationToken cancellationToken = default)
    {
        return await _context.Ufs
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.IdUf == idUf, cancellationToken);
    }

    public async Task<UfModel?> AlterarUfAsync(UfModel model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Ufs
            .SingleOrDefaultAsync(item => item.IdUf == model.IdUf, cancellationToken);

        if (entity is null)
            return null;
        
        entity.SiglaUf = NormalizarSigla(model.SiglaUf);
        entity.Sigla = model.Sigla?.Trim();

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> ApagarUfAsync(int idUf, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Ufs
            .SingleOrDefaultAsync(item => item.IdUf == idUf, cancellationToken);

        if (entity is null)
            return false;
        
        _context.Ufs.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string? NormalizarSigla(string? sigla)
    {
        return string.IsNullOrWhiteSpace(sigla)
            ? null
            : sigla.Trim().ToUpperInvariant();
    }
}
