using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class CodigoCConfig : IEntityTypeConfiguration<CodigoC>
{
  public void Configure(EntityTypeBuilder<CodigoC> b)
  {
    b.ToTable("codigo_c");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasData(
        new CodigoC { Id = 1, Nombre = "CodigoC 1" },
        new CodigoC { Id = 2, Nombre = "CodigoC 2" },
        new CodigoC { Id = 3, Nombre = "CodigoC 3" },
        new CodigoC { Id = 4, Nombre = "CodigoC 4" }
    );
  }
}