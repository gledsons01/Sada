using Microsoft.EntityFrameworkCore;
using Sada.Api.Entity.Context;
using Sada.Api.Entity.Model;

namespace Sada.Entity.Test;

public sealed class SadaDbContextTests
{
    [Fact]
    public void OnModelCreating_DeveConfigurarChavePrimariaDeTitulo()
    {
        using var context = CriarContexto();

        var entityType = context.Model.FindEntityType(typeof(TituloModel));
        var primaryKey = entityType?.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        var property = Assert.Single(primaryKey.Properties);
        Assert.Equal(nameof(TituloModel.IdTitulo), property.Name);
    }

    [Theory]
    [InlineData(nameof(TituloModel.Titulo), 150)]
    [InlineData(nameof(TituloModel.Descricao), 200)]
    public void OnModelCreating_DeveConfigurarCamposObrigatoriosComTamanhoMaximo(string propertyName, int maxLength)
    {
        using var context = CriarContexto();

        var property = context.Model
            .FindEntityType(typeof(TituloModel))
            ?.FindProperty(propertyName);

        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.Equal(maxLength, property.GetMaxLength());
    }

    [Fact]
    public void Titulos_DeveRetornarDbSetDeTitulo()
    {
        using var context = CriarContexto();

        Assert.NotNull(context.Titulos);
        Assert.IsAssignableFrom<DbSet<TituloModel>>(context.Titulos);
    }

    private static SadaDbContext CriarContexto()
    {
        var options = new DbContextOptionsBuilder<SadaDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SadaDbContext(options);
    }
}
