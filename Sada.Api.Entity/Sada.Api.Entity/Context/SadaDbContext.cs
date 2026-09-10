using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Model;

namespace Sada.Api.Entity.Context;

public class SadaDbContext(DbContextOptions<SadaDbContext> options) : DbContext(options)
{
    public DbSet<TituloModel> Titulos => Set<TituloModel>();
    public DbSet<UsuarioModel> Usuarios => Set<UsuarioModel>();
    public DbSet<SexoModel> Sexos => Set<SexoModel>();
    public DbSet<UfModel> Ufs => Set<UfModel>();
    public DbSet<CidadeModel> Cidades => Set<CidadeModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TituloModel>(entity =>
        {
            entity.ToTable("TBL_TITULO", "dbo");
            entity.HasKey(item => item.IdTitulo);
            entity.Property(item => item.IdTitulo)
                .HasColumnName("ID_TITULO")
                .ValueGeneratedNever();
            entity.Property(item => item.Titulo)
                .HasColumnName("TITULO")
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(item => item.Descricao)
                .HasColumnName("DESCRICAO")
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(item => item.Vencimento)
                .HasColumnName("DATA_VENCIMENTO")
                .HasColumnType("date");
            entity.Property(item => item.Status)
                .HasColumnName("STATUS")
                .HasColumnType("char(1)");
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

        modelBuilder.Entity<SexoModel>(entity =>
        {
            entity.ToTable("TBL_SEXO", "dbo");
            entity.HasKey(item => item.IdSexo);
            entity.Property(item => item.IdSexo)
                .HasColumnName("ID_SEXO")
                .ValueGeneratedNever();
            entity.Property(item => item.Descricao)
                .HasColumnName("DESCRICAO")
                .HasMaxLength(30)
                .IsUnicode(false)
                .IsRequired();
            entity.Property(item => item.Sigla)
                .HasColumnName("SIGLA")
                .HasColumnType("char(2)")
                .IsRequired();
        });

        modelBuilder.Entity<UfModel>(entity =>
        {
            entity.ToTable("TBL_UF", "dbo");
            entity.HasKey(item => item.IdUf);
            entity.Property(item => item.IdUf)
                .HasColumnName("ID_UF")
                .ValueGeneratedNever();
            entity.Property(item => item.SiglaUf)
                .HasColumnName("SIGLA_UF")
                .HasColumnType("char(2)");
            entity.Property(item => item.Sigla)
                .HasColumnName("DESCRICAO_IF")
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<CidadeModel>(entity =>
        {
            entity.ToTable("TBL_CIDADE", "dbo");
            entity.HasKey(item => item.IdCidade);
            entity.Property(item => item.IdCidade)
                .HasColumnName("ID_CIDADE")
                .ValueGeneratedNever();
            entity.Property(item => item.IdUf)
                .HasColumnName("ID_UF");
            entity.Property(item => item.DescricaoCidade)
                .HasColumnName("DESCRICAO_CIDADE")
                .HasMaxLength(300)
                .IsUnicode(false);

            entity.HasOne<UfModel>()
                .WithMany()
                .HasForeignKey(item => item.IdUf)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_TBL_CIDADE_TBL_UF");
        });

        base.OnModelCreating(modelBuilder);
    }
}



