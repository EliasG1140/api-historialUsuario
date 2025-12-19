using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class PuestoVotacionConfig : IEntityTypeConfiguration<PuestoVotacion>
{
  public void Configure(EntityTypeBuilder<PuestoVotacion> b)
  {
    b.ToTable("puesto_votacion");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasIndex(x => x.Nombre).IsUnique();

    b.HasData(
        new PuestoVotacion { Id = 1, Nombre = "PuestoVotacion 1" },
        new PuestoVotacion { Id = 2, Nombre = "PuestoVotacion 2" },
        new PuestoVotacion { Id = 3, Nombre = "PuestoVotacion 3" },
        new PuestoVotacion { Id = 4, Nombre = "PuestoVotacion 4" }
    );
  }
}