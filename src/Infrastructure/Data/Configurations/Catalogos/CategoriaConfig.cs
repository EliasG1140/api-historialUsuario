using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class CategoriaConfig : IEntityTypeConfiguration<Categoria>
{
  public void Configure(EntityTypeBuilder<Categoria> b)
  {
    b.ToTable("categoria");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasData(
      new Categoria { Id = 1, Nombre = "Grupo A", Minimo = 0, Maximo = 9 },
      new Categoria { Id = 2, Nombre = "Grupo B", Minimo = 10, Maximo = 15 },
      new Categoria { Id = 3, Nombre = "Grupo C", Minimo = 16, Maximo = 30 },
      new Categoria { Id = 4, Nombre = "Grupo D", Minimo = 31, Maximo = 60 },
      new Categoria { Id = 5, Nombre = "Grupo E", Minimo = 61, Maximo = 100 },
      new Categoria { Id = 6, Nombre = "Grupo F", Minimo = 101, Maximo = 300 }
    );
  }
}