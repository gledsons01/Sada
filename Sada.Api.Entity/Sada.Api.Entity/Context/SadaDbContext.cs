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
            entity.ToTable("TBL_USUARIO");
            entity.HasKey(item => item.IdUsuario);
            entity.Property(item => item.IdUsuario)
                .HasColumnName("ID_USUARIO");
            entity.Property(item => item.NomeUsuario)
                .HasColumnName("NOME_USUARIO")
                .HasMaxLength(200);
            entity.Property(item => item.Login)
                .HasColumnName("LOGIN")
                .HasMaxLength(20);
            entity.Property(item => item.Senha)
                .HasColumnName("SENHA")
                .HasMaxLength(10);
            entity.Property(item => item.Endereco)
                .HasColumnName("ENDERECO")
                .HasMaxLength(500);
            entity.Property(item => item.NumeroEndereco)
                .HasColumnName("NUMERO")
                .HasMaxLength(20);
            entity.Property(item => item.Bairro)
                .HasColumnName("BAIRRO")
                .HasMaxLength(100);
            entity.Property(item => item.IdUf)
                .HasColumnName("ID_UF");
            entity.Property(item => item.IdCidade)
                .HasColumnName("ID_CIDADE");
            entity.Property(item => item.NomeSocial)
                .HasColumnName("NOME_SOCIAL")
                .HasMaxLength(500);
            entity.Property(item => item.ID_SEXO)
                .HasColumnName("ID_SEXO");
            entity.Property(item => item.E_MAIL)
                .HasColumnName("E_MAIL")
                .HasMaxLength(500);
        });

        base.OnModelCreating(modelBuilder);
    }
}
