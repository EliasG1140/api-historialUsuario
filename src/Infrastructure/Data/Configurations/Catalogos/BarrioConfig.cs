using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class BarrioConfig : IEntityTypeConfiguration<Barrio>
{
  public void Configure(EntityTypeBuilder<Barrio> b)
  {
    b.ToTable("barrio");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasData(
        new Barrio { Id = 1, Nombre = "Barrio 1" },
        new Barrio { Id = 2, Nombre = "Barrio 2" },
        new Barrio { Id = 3, Nombre = "Barrio 3" },
        new Barrio { Id = 4, Nombre = "Barrio 4" }
    );
  }
}