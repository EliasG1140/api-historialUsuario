using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class CodigoBConfig : IEntityTypeConfiguration<CodigoB>
{
  public void Configure(EntityTypeBuilder<CodigoB> b)
  {
    b.ToTable("codigo_b");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasMany(x => x.PersonasCodigoB)
      .WithOne(pc => pc.CodigoB)
      .HasForeignKey(pc => pc.CodigoBId)
      .OnDelete(DeleteBehavior.Cascade);

    b.HasData(
        new CodigoB { Id = 1, Nombre = "CodigoB 1" },
        new CodigoB { Id = 2, Nombre = "CodigoB 2" },
        new CodigoB { Id = 3, Nombre = "CodigoB 3" },
        new CodigoB { Id = 4, Nombre = "CodigoB 4" }
    );
  }
}