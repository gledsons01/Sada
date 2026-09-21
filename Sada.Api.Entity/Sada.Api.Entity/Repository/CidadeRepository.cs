using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Interface;
using Sada.Api.Entity.Model;
using Sada.Api.Entity.Model.Request;

namespace Sada.Api.Entity.Repository;

public class CidadeRepository(SadaDbContext context) : ICidadeRepository
{
    private readonly SadaDbContext _context = context;

    public async Task<CidadeModel> IncluirCidadeAsync(CidadeModelRequest model, CancellationToken cancellationToken = default)
    {
        var nextId = await _context.Cidades
            .Select(item => (int?)item.IdCidade)
            .MaxAsync(cancellationToken) ?? 0;

        var entity = new CidadeModel
        {
            IdCidade = nextId + 1,
            IdUf = model.IdUf,
            DescricaoCidade = NormalizarDescricao(model.DescricaoCidade)
        };

        await _context.Cidades.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<List<CidadeModel>> ListarCidadesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .AsNoTracking()
            .OrderBy(item => item.IdCidade)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<CidadeModel>> ListarCidadesPorUfAsync(int idUf, CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .AsNoTracking()
            .Where(item => item.IdUf == idUf)
            .OrderBy(item => item.DescricaoCidade)
            .ToListAsync(cancellationToken);
    }

    public async Task<CidadeModel?> ObterCidadePorIdAsync(int idCidade, CancellationToken cancellationToken = default)
    {
        return await _context.Cidades
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.IdCidade == idCidade, cancellationToken);
    }

    public async Task<CidadeModel?> AlterarCidadeAsync(CidadeModelRequest model, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Cidades
            .SingleOrDefaultAsync(item => item.IdCidade == model.IdCidade, cancellationToken);

        if (entity is null)
            return null;
        
        entity.IdUf = model.IdUf;
        entity.DescricaoCidade = NormalizarDescricao(model.DescricaoCidade);

        await _context.SaveChangesAsync(cancellationToken);

        return entity;
    }

    public async Task<bool> ApagarCidadeAsync(int idCidade, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Cidades
            .SingleOrDefaultAsync(item => item.IdCidade == idCidade, cancellationToken);

        if (entity is null)
        {
            return false;
        }

        _context.Cidades.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    private static string? NormalizarDescricao(string? descricao)
    {
        return string.IsNullOrWhiteSpace(descricao)
            ? null
            : descricao.Trim();
    }
}
