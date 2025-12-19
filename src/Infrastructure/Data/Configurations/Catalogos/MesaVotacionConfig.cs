using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Catalogos;

public class MesaVotacionConfig : IEntityTypeConfiguration<MesaVotacion>
{
  public void Configure(EntityTypeBuilder<MesaVotacion> b)
  {
    b.ToTable("mesa_votacion");
    b.HasKey(x => x.Id);
    b.Property(x => x.Nombre).HasMaxLength(150).IsRequired();

    b.HasData(
        new MesaVotacion { Id = 1, Nombre = "Mesa 1", PuestoVotacionId = 1 },
        new MesaVotacion { Id = 2, Nombre = "Mesa 2", PuestoVotacionId = 1 },
        new MesaVotacion { Id = 3, Nombre = "Mesa 3", PuestoVotacionId = 1 },
        new MesaVotacion { Id = 4, Nombre = "Mesa 4", PuestoVotacionId = 1 },
        new MesaVotacion { Id = 5, Nombre = "Mesa 1", PuestoVotacionId = 2 },
        new MesaVotacion { Id = 6, Nombre = "Mesa 2", PuestoVotacionId = 2 },
        new MesaVotacion { Id = 7, Nombre = "Mesa 3", PuestoVotacionId = 2 },
        new MesaVotacion { Id = 8, Nombre = "Mesa 4", PuestoVotacionId = 2 },
        new MesaVotacion { Id = 9, Nombre = "Mesa 1", PuestoVotacionId = 3 },
        new MesaVotacion { Id = 10, Nombre = "Mesa 2", PuestoVotacionId = 3 },
        new MesaVotacion { Id = 11, Nombre = "Mesa 3", PuestoVotacionId = 3 },
        new MesaVotacion { Id = 12, Nombre = "Mesa 4", PuestoVotacionId = 3 },
        new MesaVotacion { Id = 13, Nombre = "Mesa 1", PuestoVotacionId = 4 },
        new MesaVotacion { Id = 14, Nombre = "Mesa 2", PuestoVotacionId = 4 },
        new MesaVotacion { Id = 15, Nombre = "Mesa 3", PuestoVotacionId = 4 },
        new MesaVotacion { Id = 16, Nombre = "Mesa 4", PuestoVotacionId = 4 }
    );
  }
}