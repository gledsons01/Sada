using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Model;

namespace Sada.Api.Entity.Context;

public class SadaDbContext(DbContextOptions<SadaDbContext> options) : DbContext(options)
{
    public DbSet<TituloModel> Titulos => Set<TituloModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TituloModel>(entity =>
        {
            entity.HasKey(item => item.IdTitulo);
            entity.Property(item => item.Titulo)
                .HasMaxLength(150)
                .IsRequired();
            entity.Property(item => item.Descricao)
                .HasMaxLength(200)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
