using Domain.Personas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Personas;

public class PersonaCodigoBConfig : IEntityTypeConfiguration<PersonaCodigoB>
{
    public void Configure(EntityTypeBuilder<PersonaCodigoB> builder)
    {
        builder.HasKey(pc => new { pc.PersonaId, pc.CodigoBId });

        builder.HasOne(pc => pc.Persona)
            .WithMany(p => p.CodigosB)
            .HasForeignKey(pc => pc.PersonaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.CodigoB)
            .WithMany(cb => cb.PersonasCodigoB)
            .HasForeignKey(pc => pc.CodigoBId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
