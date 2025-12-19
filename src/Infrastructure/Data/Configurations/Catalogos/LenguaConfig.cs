using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class LenguaConfig : IEntityTypeConfiguration<Lengua>
{
  public void Configure(EntityTypeBuilder<Lengua> b)
  {
    b.ToTable("lengua");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasData(
        new Lengua { Id = 1, Nombre = "Lengua 1" },
        new Lengua { Id = 2, Nombre = "Lengua 2" },
        new Lengua { Id = 3, Nombre = "Lengua 3" },
        new Lengua { Id = 4, Nombre = "Lengua 4" }
    );
  }
}