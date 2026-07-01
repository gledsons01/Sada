using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Model;

namespace Sada.Api.Entity.Context;

public class SadaDbContext(DbContextOptions<SadaDbContext> options) : DbContext(options)
{
    public DbSet<TituloModel> Titulos => Set<TituloModel>();
    public DbSet<UsuarioModel> Usuarios => Set<UsuarioModel>();

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

        modelBuilder.Entity<UsuarioModel>(entity =>
        {
            entity.HasKey(item => item.IdUsuario);
            entity.Property(item => item.NomeUsuario)
                .HasMaxLength(200)
                .IsRequired();
            entity.Property(item => item.Login)
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(item => item.Senha)
                .HasMaxLength(10)
                .IsRequired();
            entity.Property(item => item.Endereco)
                .HasMaxLength(500)
                .IsRequired();
            entity.Property(item => item.NumeroEndereco)
                .HasMaxLength(20)
                .IsRequired();
            entity.Property(item => item.Bairro)
                .HasMaxLength(100)
                .IsRequired();
            entity.Property(item => item.NomeSocial)
                .HasMaxLength(500)
                .IsRequired();
            entity.Property(item => item.EMail)
                .HasMaxLength(500)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}
