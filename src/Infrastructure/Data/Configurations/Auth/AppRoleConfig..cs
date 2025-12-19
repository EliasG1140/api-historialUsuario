using Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth;

public class AppRoleConfig : IEntityTypeConfiguration<AppRole>
{
  public void Configure(EntityTypeBuilder<AppRole> b)
  {
    b.ToTable("role");
    b.Property(x => x.Name).HasMaxLength(256);
    b.Property(x => x.NormalizedName).HasMaxLength(256);
    b.HasIndex(x => x.NormalizedName).IsUnique();

    b.HasData(
      new AppRole
      {
        Id = Guid.Parse("019a4721-233e-78b7-b7af-c71a11a344e4"),
        Name = "Administrador",
        NormalizedName = "ADMINISTRADOR"
      },
      new AppRole
      {
        Id = Guid.Parse("072de9db-ea58-418f-a43d-39f846821b4e"),
        Name = "Digitalizador",
        NormalizedName = "DIGITALIZADOR"
      }
    );
  }
}