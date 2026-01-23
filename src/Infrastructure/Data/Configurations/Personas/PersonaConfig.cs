using Domain.Personas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Personas;

public class PersonaConfig : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nombre)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Apellido)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Cedula)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Telefono)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Direccion)
            .HasMaxLength(200);

        builder.Property(p => p.Descripcion)
            .HasMaxLength(500);

        builder.HasOne(p => p.Barrio)
            .WithMany(b => b.Personas)
            .HasForeignKey(p => p.BarrioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.CodigoC)
            .WithMany(c => c.Personas)
            .HasForeignKey(p => p.CodigoCId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Lider)
            .WithMany(l => l.PersonasACargo)
            .HasForeignKey(p => p.LiderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Coordinador)
            .WithMany(c => c.Coordinados)
            .HasForeignKey(p => p.CoordinadorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Auditoría
        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.CreatedByUserId)
            .HasMaxLength(450);

        builder.Property(p => p.LastModifiedAt);

        builder.Property(p => p.LastModifiedByUserId)
            .HasMaxLength(450);
    }
}
