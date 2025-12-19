using Domain.Personas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Personas;

public class PersonaLenguaConfig : IEntityTypeConfiguration<PersonaLengua>
{
  public void Configure(EntityTypeBuilder<PersonaLengua> builder)
  {
    builder.HasKey(x => new { x.PersonaId, x.LenguaId });

    builder.HasOne(x => x.Persona)
          .WithMany(p => p.Lenguas)
          .HasForeignKey(x => x.PersonaId);

    builder.HasOne(x => x.Lengua)
          .WithMany(l => l.PersonasLengua)
          .HasForeignKey(x => x.LenguaId);
  }
}